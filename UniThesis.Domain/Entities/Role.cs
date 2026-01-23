using UniThesis.Domain.Common.Primitives;

namespace UniThesis.Domain.Entities
{
    public class Role : Entity<int>
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string? Permissions { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Role() { }

        public static Role Create(int id, string name, string? description = null, string? permissions = null)
        {
            return new Role
            {
                Id = id,
                Name = name,
                Description = description,
                Permissions = permissions,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(string name, string? description = null, string? permissions = null)
        {
            Name = name;
            Description = description;
            Permissions = permissions;
        }
    }
}
