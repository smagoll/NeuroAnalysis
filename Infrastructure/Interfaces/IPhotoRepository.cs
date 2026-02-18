using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IPhotoRepository
{
    Task AddAsync(Photo photo);
    Task<Photo?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
}