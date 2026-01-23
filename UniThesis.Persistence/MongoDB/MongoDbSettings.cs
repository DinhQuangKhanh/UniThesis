namespace UniThesis.Persistence.MongoDB
{
    /// <summary>
    /// MongoDB connection settings.
    /// </summary>
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = "UniThesisLogs";
    }
}
