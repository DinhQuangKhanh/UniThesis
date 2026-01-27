namespace UniThesis.Domain.Common.Exceptions
{
    public class NotFoundException : DomainException
    {
        /// <summary>
        /// Gets the type of resource that was not found.
        /// </summary>
        public string ResourceType { get; }

        /// <summary>
        /// Gets the identifier of the resource that was not found.
        /// </summary>
        public string? ResourceId { get; }

        /// <summary>
        /// Initializes a new instance of the NotFoundException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public NotFoundException(string message)
            : base(message, "NOT_FOUND")
        {
            ResourceType = "Resource";
        }

        /// <summary>
        /// Initializes a new instance of the NotFoundException class.
        /// </summary>
        /// <param name="resourceType">The type of resource not found.</param>
        /// <param name="resourceId">The identifier of the resource.</param>
        public NotFoundException(string resourceType, string resourceId)
            : base($"{resourceType} with identifier '{resourceId}' was not found.", "NOT_FOUND")
        {
            ResourceType = resourceType;
            ResourceId = resourceId;
        }

        /// <summary>
        /// Initializes a new instance of the NotFoundException class.
        /// </summary>
        /// <param name="resourceType">The type of resource not found.</param>
        /// <param name="resourceId">The identifier of the resource.</param>
        public NotFoundException(string resourceType, object resourceId)
            : base($"{resourceType} with identifier '{resourceId}' was not found.", "NOT_FOUND")
        {
            ResourceType = resourceType;
            ResourceId = resourceId.ToString();
        }

        /// <summary>
        /// Creates a NotFoundException for a specific entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="id">The entity identifier.</param>
        /// <returns>A new NotFoundException instance.</returns>
        public static NotFoundException For<T>(object id)
        {
            return new NotFoundException(typeof(T).Name, id);
        }
    }
}
