using Brainbay.RickAndMorty.Application.Dtos.Response;
using Brainbay.RickAndMorty.Domain.Entities;

namespace Brainbay.RickAndMorty.Application.Mappers;

public static class LocationMapper
{
    public static LocationResponse? ToLocationResponse(this Location? location)
    {
        if (location == null)
            return null;
        
        return new LocationResponse
        {
            Id = location.Id,
            Name = location.Name,
        };
    } 
}