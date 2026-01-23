using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using UniThesis.Domain.Aggregates.MeetingAggregate.ValueObjects;
using UniThesis.Domain.Enums.Meeting;

namespace UniThesis.Persistence.ValueConverters
{
    /// <summary>
    /// Converts MeetingLocation to/from JSON string for database storage.
    /// </summary>
    public class MeetingLocationConverter : ValueConverter<MeetingLocation, string>
    {
        public MeetingLocationConverter()
            : base(
                location => JsonSerializer.Serialize(new MeetingLocationDto((int)location.Type, location.Value), JsonOptions),
                json => ConvertFromJson(json))
        { }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private static MeetingLocation ConvertFromJson(string json)
        {
            var dto = JsonSerializer.Deserialize<MeetingLocationDto>(json, JsonOptions);
            var type = (MeetingType)dto!.Type;
            return type == MeetingType.Online
                ? MeetingLocation.Online(dto.Value)
                : MeetingLocation.Offline(dto.Value);
        }

        private record MeetingLocationDto(int Type, string Value);
    }
}
