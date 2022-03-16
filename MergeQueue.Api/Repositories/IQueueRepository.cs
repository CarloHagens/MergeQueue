using System.Collections.Generic;
using MergeQueue.Api.Entities;

namespace MergeQueue.Api.Repositories
{
    public interface IQueueRepository
    {
        List<User> GetUsersForChannel(string channelId);
        bool AddUser(User userToAdd);
        bool RemoveUser(User userToRemove);
        void Jump(User user);
    }
}
