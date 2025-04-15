
namespace Brainbay.RickAndMorty.Domain.Entities;

public class CharacterEpisode
{
    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = default!;

    public Guid EpisodeId { get; set; }
    public Episode Episode { get; set; } = default!;
}
