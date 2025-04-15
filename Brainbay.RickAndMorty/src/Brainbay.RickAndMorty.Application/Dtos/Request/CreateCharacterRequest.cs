namespace Brainbay.RickAndMorty.Application.Dtos.Request;

public class CreateCharacterRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Status { get; set; }
    public string Species { get; set; }
    public string Type { get; set; }
    public string Gender { get; set; }
    public string ImageUrl { get; set; }
    
    public Guid OriginLocationId { get; set; }
    
    public Guid LocationId { get; set; }

    public List<Guid> Episodes { get; set; } = new List<Guid>();
}