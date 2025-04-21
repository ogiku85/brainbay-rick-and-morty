
using Brainbay.RickAndMorty.Domain.Enums;

namespace Brainbay.RickAndMorty.Domain.Entities;

public class Character
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int? ExternalId { get; set; }

    public string Name { get; set; }
    public CharacterStatus Status { get; set; }
    public string Species { get; set; }
    public string Type { get; set; }
    public string Gender { get; set; }
    public string ImageUrl { get; set; }
    public string? ExternalUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid? OriginId { get; set; }
    public Location? Origin { get; set; }

    public Guid? LocationId { get; set; }
    public Location? Location { get; set; }

    public ICollection<CharacterEpisode> CharacterEpisodes { get; set; } = new List<CharacterEpisode>();
}
