namespace MergeQueue.Settings
{
    public class MongoDbSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ConnectionString => $"mongodb://{User}:{Password}@{Host}:{Port}";
    }
}
