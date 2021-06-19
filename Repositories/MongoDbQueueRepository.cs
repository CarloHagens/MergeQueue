using System.Collections.Generic;
using MergeQueue.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MergeQueue.Repositories
{
    [BsonIgnoreExtraElements]
    public class MongoDbQueueRepository : IQueueRepository
    {
        private const string DatabaseName = "queue";
        private const string CollectionName = "users";
        private readonly IMongoCollection<User> userCollection;

        public MongoDbQueueRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(DatabaseName);
            userCollection = database.GetCollection<User>(CollectionName);
        }

        public List<User> GetUsersForChannel(string channelId)
        {
            var builder = Builders<User>.Filter;
            var filter = builder.Eq(user => user.ChannelId, channelId);
            return userCollection.Find(filter).ToList();
        }

        public bool AddUser(User userToAdd)
        {
            var builder = Builders<User>.Filter;
            var filter = builder.And(
                builder.Eq(user => user.ChannelId, userToAdd.ChannelId),
                builder.Eq(user => user.UserId, userToAdd.UserId));
            if (userCollection.Find(filter).Any())
            {
                return false;
            }
            userCollection.InsertOne(userToAdd);
            return true;
        }

        public bool RemoveUser(User userToRemove)
        {
            var builder = Builders<User>.Filter;
            var filter = builder.And(
                builder.Eq(user => user.ChannelId, userToRemove.ChannelId),
                builder.Eq(user => user.UserId, userToRemove.UserId));

            var foundUsers = userCollection.Find(filter).ToList();

            if (foundUsers.Count == 0)
            {
                return false;
            }

            userCollection.DeleteOne(filter);
            return true;
        }
    }
}
