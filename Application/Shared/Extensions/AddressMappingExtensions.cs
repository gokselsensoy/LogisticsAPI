using Application.Shared.Models;
using Domain.ValueObjects;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Application.Shared.Extensions
{
    public static class AddressMappingExtensions
    {
        public static Address ToValueObject(this AddressDto dto)
        {
            // Point Oluşturma (SRID 4326 - GPS)
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var location = geometryFactory.CreatePoint(new Coordinate(dto.Longitude, dto.Latitude));

            return new Address(
                dto.Street,
                dto.BuildingNo,
                dto.ZipCode,
                dto.City,
                dto.Country, // Dto'da yoksa varsayılan atanır veya null gider
                location,
                dto.Floor,
                dto.Door
            );
        }
    }
}
