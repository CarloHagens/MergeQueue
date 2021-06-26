using MongoDB.Bson.Serialization.Attributes;

namespace MergeQueue.Repositories
{
    [BsonIgnoreExtraElements]
    public class MongoDbViewRepository
    {
        //private const string DatabaseName = "queue";
        //private const string CollectionName = "views";
        //private readonly IMongoCollection<View> viewCollection;

        //public MongoDbViewRepository(IMongoClient mongoClient)
        //{
        //    var database = mongoClient.GetDatabase(DatabaseName);
        //    viewCollection = database.GetCollection<View>(CollectionName);
        //}

        //public List<User> GetUsersForChannel(string channelId)
        //{
        //    var builder = Builders<User>.Filter;
        //    var filter = builder.Eq(user => user.ChannelId, channelId);
        //    return viewCollection.Find(filter).ToList();
        //}

        //public bool AddUser(User userToAdd)
        //{
        //    var builder = Builders<User>.Filter;
        //    var filter = builder.And(
        //        builder.Eq(user => user.ChannelId, userToAdd.ChannelId),
        //        builder.Eq(user => user.UserId, userToAdd.UserId));
        //    if (viewCollection.Find(filter).Any())
        //    {
        //        return false;
        //    }
        //    viewCollection.InsertOne(userToAdd);
        //    return true;
        //}

        //public bool RemoveUser(User userToRemove)
        //{
        //    var builder = Builders<User>.Filter;
        //    var filter = builder.And(
        //        builder.Eq(user => user.ChannelId, userToRemove.ChannelId),
        //        builder.Eq(user => user.UserId, userToRemove.UserId));

        //    var foundUsers = viewCollection.Find(filter).ToList();

        //    if (foundUsers.Count == 0)
        //    {
        //        return false;
        //    }

        //    viewCollection.DeleteOne(filter);
        //    return true;
        //}
    }
}
