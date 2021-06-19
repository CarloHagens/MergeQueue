using System.Collections.Generic;
using System.Linq;
using MergeQueue.Entities;

namespace MergeQueue.Repositories
{
    public class InMemoryQueueRepository : IQueueRepository
    {
        private readonly List<User> _queuedUsers;

        public InMemoryQueueRepository()
        {
            _queuedUsers = new List<User>();
        }

        public List<User> GetUsersForChannel(string channelId)
        {
            return _queuedUsers.Where(queuedUser => queuedUser.ChannelId == channelId).ToList();
        }

        public bool AddUser(User userToAdd)
        {
            if (_queuedUsers.Any(queuedUser => queuedUser.UserId == userToAdd.UserId 
                                                 && queuedUser.ChannelId == userToAdd.ChannelId))
            {
                return false;
            }
            _queuedUsers.Add(userToAdd);
            return true;
        }

        public bool RemoveUser(User userToRemove)
        {
            var queuedUser = _queuedUsers.FirstOrDefault(user => user.UserId == userToRemove.UserId 
                                                                 && user.ChannelId == userToRemove.ChannelId);
            if (queuedUser is null)
            {
                return false;
            }

            _queuedUsers.Remove(queuedUser);
            return true;
        }
    }
}
