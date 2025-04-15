using Brainbay.RickAndMorty.Application.Interfaces;
using Brainbay.RickAndMorty.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Brainbay.RickAndMorty.Infrastructure.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly CharacterDbContext _dbContext;

    public CharacterRepository(CharacterDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Character>> GetAllAsync()
    {
        return await _dbContext.Characters
            .Include(c => c.Origin)
            .Include(c => c.Location)
            .Include(c => c.CharacterEpisodes)
            .ThenInclude(ce => ce.Episode)
            .ToListAsync();
    }

    public async Task<IEnumerable<Character>> GetByOriginPlanetAsync(string planetName)
    {
        return await _dbContext.Characters
            .Where(c => c.Origin.Name.ToLower() == planetName.ToLower())
            .ToListAsync();
    }

    public async Task AddCharacterAsync(Character character)
    {
        await _dbContext.Characters.AddAsync(character);
    }

    public async Task<Character?> GetByNameAsync(string characterName)
    {
        return await _dbContext.Characters
            .Where(c => c.Name.ToLower() == characterName.ToLower())
            .FirstOrDefaultAsync();
    }
    
    public async Task<bool> CharacterExists(string characterName)
    {
        return await _dbContext.Characters
            .Where(c => c.Name.ToLower() == characterName.ToLower())
            .AnyAsync();
    }

    public async Task<int> SaveAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}