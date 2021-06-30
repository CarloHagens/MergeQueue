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
        private readonly IMongoCollection<User> _userCollection;

        public MongoDbQueueRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(DatabaseName);
            _userCollection = database.GetCollection<User>(CollectionName);
        }

        public List<User> GetUsersForChannel(string channelId)
        {
            var builder = Builders<User>.Filter;
            var filter = builder.Eq(user => user.ChannelId, channelId);
            return _userCollection.Find(filter).ToList();
        }

        public bool AddUser(User userToAdd)
        {
            var builder = Builders<User>.Filter;
            var filter = builder.And(
                builder.Eq(user => user.ChannelId, userToAdd.ChannelId),
                builder.Eq(user => user.UserId, userToAdd.UserId));
            if (_userCollection.Find(filter).Any())
            {
                return false;
            }
            _userCollection.InsertOne(userToAdd);
            return true;
        }

        public bool RemoveUser(User userToRemove)
        {
            var builder = Builders<User>.Filter;
            var filter = builder.And(
                builder.Eq(user => user.ChannelId, userToRemove.ChannelId),
                builder.Eq(user => user.UserId, userToRemove.UserId));

            var foundUsers = _userCollection.Find(filter).ToList();

            if (foundUsers.Count == 0)
            {
                return false;
            }

            _userCollection.DeleteOne(filter);
            return true;
        }

        public void Jump(User user)
        {
            var usersForChannel = GetUsersForChannel(user.ChannelId);
            foreach (var userInChannel in usersForChannel)
            {
                RemoveUser(userInChannel);
            }
            AddUser(user);
            foreach (var userInChannel in usersForChannel)
            {
                AddUser(userInChannel);
            }
        }
    }
}
