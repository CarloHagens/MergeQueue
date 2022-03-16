using MongoDB.Bson;

namespace MergeQueue.Api.Entities
{
    public class User
    {
        public ObjectId Id { get; init; }
        public string ChannelId { get; init; }
        public string UserId { get; init; }
    }
}
