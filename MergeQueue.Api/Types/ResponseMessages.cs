using MergeQueue.Api.Controllers;
using MergeQueue.Api.Entities;

namespace MergeQueue.Api.Types
{
    public class ResponseMessages
    {
        public static string QueueIsEmpty => "The queue is currently empty.";
        public static string AlreadyInQueue => "You are already in the queue.";
        public static string AlreadyInQueueAtThatPosition => "You are either already in the queue at that position, or the position you specified is invalid.";
        public static string NotInQueue => "You are not in the queue.";
        public static string QueueNowEmpty => " It is now empty.";
        public static string SelectChannel => "Select a channel from the list.";
        public static string SelectChannelPlaceholder => "Select channel";
        public static string SelectUser => "Select a user from the list.";
        public static string SelectUserPlaceholder => "Select user";

        public static string UserNotInQueue(string userId) => $"<@{userId}> is not in the queue.";
        public static string UserLeftQueue(string userId) => $"<@{userId}> left the queue.";
        public static string UserKickedFromTheQueue(string userId) => $"<@{userId}> was kicked from the queue.";
        public static string UserFirstInQueue(string userId) => $"<@{userId}> is now first in the queue!";
        public static string UserJoinedQueue(string userId) => $"<@{userId}> joined the queue.";
        public static string UserJumpedQueue(string userId) => $"<@{userId}> jumped the queue and is now first!";
        public static string UserQueuePosition(int queuePosition, string userId) => $"{SlackNumberEmojis.From(queuePosition + 1)} <@{userId}>";
        public static string UserTurnArrived(string userId) => $"\n<@{userId}> it is now your turn!";

        public static string QueueIntroduction => ":wave: Need some help with `/queue`?";
        public static string AvailableCommands => "*Available commands*";
        public static string QueueCommands = "" +
                                              $"`/queue {Commands.Show}` - Show a queue for this channel\n" +
                                              $"`/queue {Commands.Join}` - Join the queue\n" +
                                              $"`/queue {Commands.Join} 2` - Join the queue at the specified position\n" +
                                              $"`/queue {Commands.Leave}` - Leave the queue\n" +
                                              $"`/queue {Commands.Jump}` - Jump to the first position in the queue\n" +
                                              $"`/queue {Commands.Kick} @user` - Kick user from the queue\n" +
                                              $"`/queue {Commands.Help}` - Show all available commands";
    }
}
