using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Application.Services;

public class PhotoAnalysisService : IPhotoAnalysisService
{
    private readonly IPhotoRepository _photoRepo;
    private readonly IMaterialRepository _materialRepo;
    private readonly IPromptService _promptService;
    private readonly IDownloadService _downloadService;

    public PhotoAnalysisService(
        IPhotoRepository photoRepo,
        IMaterialRepository materialRepo,
        IPromptService promptService,
        IDownloadService downloadService)
    {
        _photoRepo = photoRepo;
        _materialRepo = materialRepo;
        _promptService = promptService;
        _downloadService = downloadService;
    }
    
    public async Task<(Guid PhotoId, List<string> Objects)> AnalyzePhotoAsync(string imageUrl)
    {
        var image = await _downloadService.DownloadImageAsync(imageUrl);

        var detectedObjects = await _promptService.DetectObjectsAsync(image);

        var photo = new Photo
        {
            Id = Guid.NewGuid(),
            Bytes = image,
            Url = imageUrl
        };

        foreach (var obj in detectedObjects)
        {
            photo.Objects.Add(new DetectedObject
            {
                Id = Guid.NewGuid(),
                Name = obj
            });
        }

        await _photoRepo.AddAsync(photo);
        await _photoRepo.SaveChangesAsync();

        return (photo.Id, detectedObjects);
    }
    
    public async Task<Dictionary<string, List<string>>> AnalyzeMaterialsAsync(Guid photoId, List<string> objects)
    {
        var photo = await _photoRepo.GetByIdAsync(photoId);
        if (photo == null)
            throw new Exception("Photo not found");

        var materialsObjects = await _promptService.AnalyzeMaterialsAsync(photo.Bytes, objects);
        var allDetectedMatNames = materialsObjects.Values.SelectMany(x => x).Distinct().ToList();
        var existingMaterials = await _materialRepo.GetByNamesAsync(allDetectedMatNames);
        var materialsByName = existingMaterials.ToDictionary(m => m.Name.ToLower());

        foreach (var detected in photo.Objects)
        {
            if (!materialsObjects.TryGetValue(detected.Name, out var materials))
                continue;

            foreach (var matName in materials)
            {
                if (!materialsByName.TryGetValue(matName.ToLower(), out var material))
                {
                    material = new Material { Id = Guid.NewGuid(), Name = matName };
                    await _materialRepo.AddAsync(material);
                    materialsByName[matName.ToLower()] = material;
                }

                if (detected.ObjectMaterials.All(om => om.MaterialId != material.Id))
                {
                    detected.ObjectMaterials.Add(new ObjectMaterial
                    {
                        DetectedObjectId = detected.Id,
                        MaterialId = material.Id
                    });
                }
            }
        }

        await _photoRepo.SaveChangesAsync();

        return materialsObjects;
    }
}