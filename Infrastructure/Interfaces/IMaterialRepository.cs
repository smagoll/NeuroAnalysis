using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IMaterialRepository
{
    Task<Material?> GetByNameAsync(string name);
    Task<List<Material>> GetByNamesAsync(List<string> name);
    Task AddAsync(Material material);
}