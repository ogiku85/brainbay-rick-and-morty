
namespace Brainbay.RickAndMorty.Domain.Entities;

public class Character
{
    public Guid Id { get; set; }
    public int? ExternalId { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public string Species { get; set; }
    public string Type { get; set; }
    public string Gender { get; set; }
    public string Image { get; set; }
    public string Url { get; set; }
    public DateTime Created { get; set; }

    public int? OriginId { get; set; }
    public Location? Origin { get; set; }
    public int? LocationId { get; set; }
    public Location? Location { get; set; }

    public ICollection<CharacterEpisode> CharacterEpisodes { get; set; } = new List<CharacterEpisode>();
}

