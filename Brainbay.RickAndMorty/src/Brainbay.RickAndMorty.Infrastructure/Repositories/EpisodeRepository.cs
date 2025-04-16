using Brainbay.RickAndMorty.Application.Interfaces;
using Brainbay.RickAndMorty.Domain.Entities;

namespace Brainbay.RickAndMorty.Infrastructure.Repositories;


public class EpisodeRepository : IEpisodeRepository
{
    private readonly CharacterDbContext _context;
    
    public EpisodeRepository(CharacterDbContext context) => _context = context;

    public Task AddRangeAsync(IEnumerable<Episode> episodes)
    {
       return _context.Episodes.AddRangeAsync(episodes);
    }

    public Task ClearAsync()
    {
        _context.Episodes.RemoveRange(_context.Episodes);
        return Task.CompletedTask;
    }
}
