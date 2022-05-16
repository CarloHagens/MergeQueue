using MergeQueue.Api.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MergeQueue.Api.Repositories
{
    [BsonIgnoreExtraElements]
    public class MongoDbQueueRepository : IQueueRepository
    {
        private const string DatabaseName = "queue";
        private const string CollectionName = "users";
        private readonly IMongoClient _mongoClient;
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);
        public MongoDbQueueRepository(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        public async Task<List<User>> GetUsersForChannel(string channelId)
        {
            var users = await GetUserCollection().FindAsync(user => user.ChannelId == channelId);
            return users.ToList();
        }

        public async Task<bool> AddUser(User userToAdd)
        {
            var builder = Builders<User>.Update;
            var updateDefinition = builder
                .Set(user => user.UserId, userToAdd.UserId)
                .Set(user => user.ChannelId, userToAdd.ChannelId);

            var updateResult = await GetUserCollection().UpdateOneAsync(
                user => user.ChannelId == userToAdd.ChannelId
                && user.UserId == userToAdd.UserId,
                updateDefinition,
                new UpdateOptions { IsUpsert = true }
            );

            return updateResult.IsAcknowledged && updateResult.MatchedCount == 0;
        }

        public async Task<bool> RemoveUser(User userToRemove)
        {
            var deleteResult = await GetUserCollection().DeleteOneAsync(
                user => user.ChannelId == userToRemove.ChannelId
                && user.UserId == userToRemove.UserId
            );

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount == 1;
        }

        public async Task Jump(User user)
        {
            using var session = await _mongoClient.StartSessionAsync();
            session.StartTransaction();
            try
            {
                var usersForChannel = await GetUsersForChannel(user.ChannelId);
                foreach (var userInChannel in usersForChannel)
                {
                    await RemoveUser(userInChannel);
                }
                await AddUser(user);
                foreach (var userInChannel in usersForChannel)
                {
                    await AddUser(userInChannel);
                }
            }
            catch (Exception)
            {
                await session.AbortTransactionAsync();
            }
            await session.CommitTransactionAsync();
        }

        private IMongoCollection<User> GetUserCollection()
        {
            var database = _mongoClient.GetDatabase(DatabaseName);
            return database.GetCollection<User>(CollectionName);
        }
    }
}
