using Brainbay.RickAndMorty.Domain.Entities;

namespace Brainbay.RickAndMorty.Application.Interfaces;

public interface IEpisodeRepository
{
    Task AddRangeAsync(IEnumerable<Episode> episodes);
    Task ClearAsync();
}
