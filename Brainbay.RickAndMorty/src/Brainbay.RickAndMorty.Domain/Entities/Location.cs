
namespace Brainbay.RickAndMorty.Domain.Entities;

public class Location
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int? ExternalId { get; set; } // nullable if external ID is not available
    public string Name { get; set; }
    public string Url { get; set; }
}
