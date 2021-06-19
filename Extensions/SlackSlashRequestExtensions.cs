using MergeQueue.Dtos;
using MergeQueue.Entities;

namespace MergeQueue.Extensions
{
    public static class SlackSlashRequestExtensions
    {
        public static User ToUser(this SlackSlashRequestDto slackSlashCommand)
        {
            return new()
            {
                ChannelId = slackSlashCommand.channel_id,
                UserId = slackSlashCommand.user_id
            };
        }

        public static string GetCommand(this SlackSlashRequestDto slackSlashCommand)
            => slackSlashCommand.text?.ToLowerInvariant();
    }
}
