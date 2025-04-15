
namespace Brainbay.RickAndMorty.Domain.Entities;

public class Location
{
    public Guid Id { get; set; }
    public int? ExternalId { get; set; }
    public string Name { get; set; } = default!;
    public string Url { get; set; } = default!;
}
