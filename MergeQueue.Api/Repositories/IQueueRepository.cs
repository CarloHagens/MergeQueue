using MergeQueue.Api.Entities;

namespace MergeQueue.Api.Repositories
{
    public interface IQueueRepository
    {
        Task<List<User>> GetUsersForChannel(string channelId);
        Task<bool> AddUser(User userToAdd);
        Task<bool> RemoveUser(User userToRemove);
        Task Jump(User user);
    }
}
