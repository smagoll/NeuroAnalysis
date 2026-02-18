namespace Domain.DTOs;

public record AnalyzeMaterialsRequest(Guid PhotoId, List<string> Objects);
public record AnalyzeMaterialsResponse(Dictionary<string, List<string>> Materials);