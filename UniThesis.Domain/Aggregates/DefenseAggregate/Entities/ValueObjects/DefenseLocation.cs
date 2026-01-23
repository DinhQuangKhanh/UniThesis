using UniThesis.Domain.Common.Primitives;

namespace UniThesis.Domain.Aggregates.DefenseAggregate.Entities.ValueObjects
{
    public sealed class DefenseLocation : ValueObject
    {
        public string Room { get; }
        public string? Building { get; }
        public string? Address { get; }

        private DefenseLocation(string room, string? building, string? address)
        {
            Room = room;
            Building = building;
            Address = address;
        }

        public static DefenseLocation Create(string room, string? building = null, string? address = null)
        {
            if (string.IsNullOrWhiteSpace(room))
                throw new ArgumentException("Room cannot be empty.", nameof(room));
            return new DefenseLocation(room.Trim(), building?.Trim(), address?.Trim());
        }

        public override string ToString() => Building != null ? $"{Room}, {Building}" : Room;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Room;
            yield return Building;
            yield return Address;
        }
    }
}
