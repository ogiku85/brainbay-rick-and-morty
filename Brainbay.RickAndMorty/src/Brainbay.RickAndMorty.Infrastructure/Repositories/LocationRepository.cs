using Brainbay.RickAndMorty.Application.Interfaces;
using Brainbay.RickAndMorty.Domain.Entities;

namespace Brainbay.RickAndMorty.Infrastructure.Repositories;


public class LocationRepository : ILocationRepository
{
    private readonly CharacterDbContext _context;
    
    public LocationRepository(CharacterDbContext context) => _context = context;

    public Task AddRangeAsync(IEnumerable<Location> locations)
    {
       return _context.Locations.AddRangeAsync(locations);
    }

    public Task ClearAsync()
    {
        _context.Locations.RemoveRange(_context.Locations);
        return Task.CompletedTask;
    }
}
