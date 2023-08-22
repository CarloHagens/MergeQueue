using MergeQueue.Api.Entities;

namespace MergeQueue.Api.Repositories
{
    public interface IQueueLookup
    {
        Task<List<User>> GetUsersForChannel(string channelId);
        Task<bool> AddUser(User userToAdd);
        Task<bool> AddUser(User userToAdd, int position);
        Task<bool> RemoveUser(User userToRemove);
    }
}
