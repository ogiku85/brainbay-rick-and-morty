using Brainbay.RickAndMorty.Domain.Entities;

namespace Brainbay.RickAndMorty.Application.Interfaces;


public interface ILocationRepository
{
    Task AddRangeAsync(IEnumerable<Location> locations);
    Task ClearAsync();
}
