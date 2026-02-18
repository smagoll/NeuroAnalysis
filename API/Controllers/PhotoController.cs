using Application.Interfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace NeuroAnalysis.Controllers;

[ApiController]
[Route("api/photo")]
public class PhotoController : ControllerBase
{
    private readonly IPhotoAnalysisService _service;

    public PhotoController(IPhotoAnalysisService service)
    {
        _service = service;
    }
    
    [HttpPost("analyze")]
    public async Task<ActionResult<AnalyzePhotoResponse>> AnalyzePhoto([FromBody] AnalyzePhotoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ImageUrl))
            return BadRequest("ImageUrl is required");

        var objects = await _service.AnalyzePhotoAsync(request.ImageUrl);

        var response = new AnalyzePhotoResponse(
            PhotoId: objects.PhotoId,
            Objects: objects.Objects
        );

        return Ok(response);
    }
    
    [HttpPost("materials")]
    public async Task<ActionResult<AnalyzeMaterialsResponse>> AnalyzeMaterials([FromBody] AnalyzeMaterialsRequest request)
    {
        var result = await _service.AnalyzeMaterialsAsync(request.PhotoId, request.Objects);

        return Ok(new AnalyzeMaterialsResponse(Materials: result));
    }
}