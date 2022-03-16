using System.Text.RegularExpressions;
using MergeQueue.Api.Dtos;
using MergeQueue.Api.Entities;

namespace MergeQueue.Api.Extensions
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

        public static User ToUserToKick(this SlackSlashRequestDto slackSlashCommand, string userId)
        {
            return new()
            {
                ChannelId = slackSlashCommand.channel_id,
                UserId = userId
            };
        }

        public static string GetCommand(this SlackSlashRequestDto slackSlashCommand)
            => slackSlashCommand.text?.ToLowerInvariant();

        public static string GetKickCommand(this SlackSlashRequestDto slackSlashCommand)
        {
            if (slackSlashCommand.text != null
                && slackSlashCommand.text.Contains(' ')
                && (Regex.IsMatch(slackSlashCommand.text, "kick <@\\w*\\|\\w*>")
                || Regex.IsMatch(slackSlashCommand.text, "kick <@\\w*>"))
            )
            {
                return slackSlashCommand.text?.Split(' ')[0].ToLowerInvariant();
            }

            return slackSlashCommand.text;
        }
    }
}
