namespace Domain.Entities;

public class DetectedObject
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    // FK
    public Guid PhotoId { get; set; }
    public Photo Photo { get; set; } = null!;

    public ICollection<ObjectMaterial> ObjectMaterials { get; set; } = new List<ObjectMaterial>();
}