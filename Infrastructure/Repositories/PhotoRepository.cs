using Domain.Entities;
using Infrastructure.Database;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PhotoRepository : IPhotoRepository
{
    private readonly AppDbContext _context;

    public PhotoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Photo photo)
    {
        await _context.PhotoAnalyzes.AddAsync(photo);
    }

    public async Task<Photo?> GetByIdAsync(Guid id)
    {
        return await _context.PhotoAnalyzes
            .Include(p => p.Objects)
            .ThenInclude(o => o.ObjectMaterials)
            .ThenInclude(om => om.Material)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}