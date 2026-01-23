using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using UniThesis.Domain.Aggregates.DefenseAggregate.Entities.ValueObjects;

namespace UniThesis.Persistence.ValueConverters
{
    /// <summary>
    /// Converts DefenseLocation to/from JSON string for database storage.
    /// </summary>
    public class DefenseLocationConverter : ValueConverter<DefenseLocation, string>
    {
        public DefenseLocationConverter()
            : base(
                location => JsonSerializer.Serialize(new DefenseLocationDto(location.Room, location.Building, location.Address), JsonOptions),
                json => ConvertFromJson(json))
        { }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private static DefenseLocation ConvertFromJson(string json)
        {
            var dto = JsonSerializer.Deserialize<DefenseLocationDto>(json, JsonOptions);
            return DefenseLocation.Create(dto!.Room, dto.Building, dto.Address);
        }

        private record DefenseLocationDto(string Room, string? Building, string? Address);
    }
}
