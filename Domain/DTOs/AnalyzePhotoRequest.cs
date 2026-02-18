namespace Domain.DTOs;

public record AnalyzePhotoRequest(string ImageUrl);
public record AnalyzePhotoResponse(Guid PhotoId, IEnumerable<string> Objects);