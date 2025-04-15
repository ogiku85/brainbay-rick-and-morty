
namespace Brainbay.RickAndMorty.Domain.Entities;

public class Episode
{
    public Guid Id { get; set; }
    public int? ExternalId { get; set; }
    public string Url { get; set; } = default!;
    public ICollection<CharacterEpisode> CharacterEpisodes { get; set; } = new List<CharacterEpisode>();
}
