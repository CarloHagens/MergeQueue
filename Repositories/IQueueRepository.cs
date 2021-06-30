using System.Collections.Generic;
using MergeQueue.Entities;

namespace MergeQueue.Repositories
{
    public interface IQueueRepository
    {
        List<User> GetUsersForChannel(string channelId);
        bool AddUser(User userToAdd);
        bool RemoveUser(User userToRemove);
        void Jump(User user);
    }
}
