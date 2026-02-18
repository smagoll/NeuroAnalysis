namespace Domain.Entities;

public class ObjectMaterial
{
    public Guid DetectedObjectId { get; set; }
    public DetectedObject DetectedObject { get; set; } = null!;

    public Guid MaterialId { get; set; }
    public Material Material { get; set; } = null!;
}