using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace UniThesis.Persistence.MongoDB.Serializers
{
    /// <summary>
    /// Serializer configuration for MongoDB.
    /// </summary>
    public static class MongoSerializerConfiguration
    {
        private static bool _configured;

        /// <summary>
        /// Configures global serializers for MongoDB.
        /// Should be called once at application startup.
        /// </summary>
        public static void Configure()
        {
            if (_configured) return;

            // Register custom serializers
            BsonSerializer.RegisterSerializer(new GuidAsStringSerializer());
            BsonSerializer.RegisterSerializer(new NullableGuidAsStringSerializer());

            // Configure DateTime to always use UTC
            BsonSerializer.RegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeKind.Utc));

            _configured = true;
        }
    }
}
