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
        private readonly IMongoCollection<User> _userCollection;

        public MongoDbQueueRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(DatabaseName);
            _userCollection = database.GetCollection<User>(CollectionName);
        }

        public async Task<List<User>> GetUsersForChannel(string channelId)
        {
            var users = await _userCollection.FindAsync(user => user.ChannelId == channelId);
            return users.ToList();
        }

        public async Task<bool> AddUser(User userToAdd)
        {
            var builder = Builders<User>.Update;
            var updateDefinition = builder
                .Set(user => user.UserId, userToAdd.UserId)
                .Set(user => user.ChannelId, userToAdd.ChannelId);

            var updateResult = await _userCollection.UpdateOneAsync(
                user => user.ChannelId == userToAdd.ChannelId
                && user.UserId == userToAdd.UserId,
                updateDefinition,
                new UpdateOptions { IsUpsert = true }
            );

            return updateResult.IsAcknowledged && updateResult.MatchedCount == 0;
        }

        public async Task<bool> AddUser(User userToAdd, int position)
        {
            var usersForChannel = await GetUsersForChannel(userToAdd.ChannelId);
            var existingPosition = usersForChannel.FindIndex(
                user => user.UserId == userToAdd.UserId 
                && user.ChannelId == userToAdd.ChannelId);
            if (existingPosition >= 0 && position - 1 == existingPosition 
                || position > usersForChannel.Count)
            {
                return false;
            }


            foreach (var userInChannel in usersForChannel)
            {
                await RemoveUser(userInChannel);
            }

            if (existingPosition >= 0)
            {
                var existingUser = usersForChannel.Find(
                    user => user.UserId == userToAdd.UserId 
                    && user.ChannelId == userToAdd.ChannelId);
                usersForChannel.Remove(existingUser);
            }

            var newUserInserted = false;
            var usersInserted = 0;

            while (usersInserted != usersForChannel.Count + 1)
            {
                if (position == usersInserted + 1 && !newUserInserted)
                {
                    await AddUser(userToAdd);
                    newUserInserted = true;
                }
                else
                {
                    var index = newUserInserted ? usersInserted - 1 : usersInserted;
                    await AddUser(usersForChannel[index]);
                }
                usersInserted++;
            }

            return true;
        }

        public async Task<bool> RemoveUser(User userToRemove)
        {
            var deleteResult = await _userCollection.DeleteOneAsync(
                user => user.ChannelId == userToRemove.ChannelId
                && user.UserId == userToRemove.UserId
            );

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount == 1;
        }
    }
}
