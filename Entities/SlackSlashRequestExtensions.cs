using MergeQueue.Dtos;

namespace MergeQueue.Entities
{
    public static class SlackSlashRequestExtensions
    {
        public static User ToUser(this SlackSlashRequestDto slackSlashCommand)
        {
            return new User
            {
                ChannelId = slackSlashCommand.channel_id,
                UserId = slackSlashCommand.user_id
            };
        }
    }
}
