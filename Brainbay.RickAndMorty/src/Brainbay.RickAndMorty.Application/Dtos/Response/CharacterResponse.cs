using Brainbay.RickAndMorty.Domain.Enums;

namespace Brainbay.RickAndMorty.Application.Dtos.Response;

public class CharacterResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public CharacterStatus Status { get; set; }
    public string Species { get; set; }
    public string Type { get; set; }
    public string Gender { get; set; }
    public string ImageUrl { get; set; }
    
    public LocationResponse? Origin { get; set; }
    
    public LocationResponse? Location { get; set; }

    public IReadOnlyCollection<EpisodeResponse> Episodes { get; set; } = new List<EpisodeResponse>();
}