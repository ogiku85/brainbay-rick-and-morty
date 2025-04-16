using Brainbay.RickAndMorty.Application.Interfaces;

namespace Brainbay.RickAndMorty.Infrastructure.Repositories;


public class CharacterEpisodeRepository : ICharacterEpisodeRepository
{
    private readonly CharacterDbContext _context;
    
    public CharacterEpisodeRepository(CharacterDbContext context) => _context = context;

    public Task ClearAsync()
    {
        _context.CharacterEpisodes.RemoveRange(_context.CharacterEpisodes);
        return Task.CompletedTask;
    }
}
