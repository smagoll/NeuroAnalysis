namespace Domain.Entities;

public class Material
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<ObjectMaterial> ObjectMaterials { get; set; } = new List<ObjectMaterial>();
}