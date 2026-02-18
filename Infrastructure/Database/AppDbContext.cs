using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AppDbContext(DbContextOptions opts) : DbContext(opts)
{
    public DbSet<Photo> PhotoAnalyzes { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<ObjectMaterial> ObjectMaterials { get; set; }
    public DbSet<DetectedObject> DetectedObjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<ObjectMaterial>()
            .HasKey(x => new { x.DetectedObjectId, x.MaterialId });
        
        modelBuilder.Entity<ObjectMaterial>()
            .HasOne(x => x.DetectedObject)
            .WithMany(o => o.ObjectMaterials)
            .HasForeignKey(x => x.DetectedObjectId);

        modelBuilder.Entity<ObjectMaterial>()
            .HasOne(x => x.Material)
            .WithMany(m => m.ObjectMaterials)
            .HasForeignKey(x => x.MaterialId);
        
        modelBuilder.Entity<Material>()
            .HasIndex(x => x.Name)
            .IsUnique();
    }
}