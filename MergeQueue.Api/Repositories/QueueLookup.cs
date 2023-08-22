using MergeQueue.Api.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace MergeQueue.Api.Repositories
{
    public class QueueLookup : IQueueLookup
    {
        private readonly IQueueRepository queueRepository;
        private readonly IMemoryCache memoryCache;
        private readonly ILogger logger;

        public QueueLookup(IQueueRepository queueRepository, IMemoryCache memoryCache, ILogger logger)
        {
            this.queueRepository = queueRepository;
            this.memoryCache = memoryCache;
            this.logger = logger;
        }
        public async Task<bool> AddUser(User userToAdd)
        {
            logger.LogInformation($"Adding user {userToAdd.UserId} to queue.");
            await queueRepository.AddUser(userToAdd);
            logger.LogInformation($"Clearing cache for channel {userToAdd.ChannelId}.");
            memoryCache.Remove(userToAdd.ChannelId);
            return true;
        }

        public async Task<bool> AddUser(User userToAdd, int position)
        {
            logger.LogInformation($"Adding user {userToAdd.UserId} to queue.");
            await queueRepository.AddUser(userToAdd, position);
            logger.LogInformation($"Clearing cache for channel {userToAdd.ChannelId}.");
            memoryCache.Remove(userToAdd.ChannelId);
            return true;
        }

        public async Task<List<User>> GetUsersForChannel(string channelId)
        {
            if (!memoryCache.TryGetValue(channelId, out List<User> users))
            {
                logger.LogInformation($"Users for channel {channelId} not available from cache.");
                logger.LogInformation("Retrieving users from database.");
                users = await queueRepository.GetUsersForChannel(channelId);
                logger.LogInformation("Adding retrieved users to cache.");
                memoryCache.Set(channelId, users);
            }

            return users;
        }

        public async Task<bool> RemoveUser(User userToRemove)
        {
            logger.LogInformation($"Removing user {userToRemove.UserId} from queue.");
            await queueRepository.RemoveUser(userToRemove);
            logger.LogInformation($"Clearing cache for channel {userToRemove.ChannelId}.");
            memoryCache.Remove(userToRemove.ChannelId);
            return true;
        }
    }
}
