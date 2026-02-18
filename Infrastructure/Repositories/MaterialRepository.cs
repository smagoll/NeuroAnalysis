using Domain.Entities;
using Infrastructure.Database;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MaterialRepository : IMaterialRepository
{
    private readonly AppDbContext _context;

    public MaterialRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Material?> GetByNameAsync(string name)
    {
        return await _context.Materials
            .FirstOrDefaultAsync(m => m.Name.ToLower() == name.ToLower());
    }

    public async Task AddAsync(Material material)
    {
        await _context.Materials.AddAsync(material);
    }
}
