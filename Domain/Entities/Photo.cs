namespace Domain.Entities;

public class Photo
{
    public Guid Id { get; set; }
    public byte[] Bytes { get; set; }

    public string Url { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<DetectedObject> Objects { get; set; } = new List<DetectedObject>();
}