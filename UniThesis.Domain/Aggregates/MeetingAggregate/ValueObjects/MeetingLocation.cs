using UniThesis.Domain.Common.Primitives;
using UniThesis.Domain.Enums.Meeting;

namespace UniThesis.Domain.Aggregates.MeetingAggregate.ValueObjects
{
    public sealed class MeetingLocation : ValueObject
    {
        public MeetingType Type { get; }
        public string Value { get; }

        private MeetingLocation(MeetingType type, string value)
        {
            Type = type;
            Value = value;
        }

        public static MeetingLocation Online(string meetingLink)
        {
            if (string.IsNullOrWhiteSpace(meetingLink))
                throw new ArgumentException("Meeting link cannot be empty.", nameof(meetingLink));
            return new MeetingLocation(MeetingType.Online, meetingLink.Trim());
        }

        public static MeetingLocation Offline(string room)
        {
            if (string.IsNullOrWhiteSpace(room))
                throw new ArgumentException("Room cannot be empty.", nameof(room));
            return new MeetingLocation(MeetingType.Offline, room.Trim());
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Type;
            yield return Value;
        }

        public override string ToString() => $"{Type}: {Value}";
    }
}
