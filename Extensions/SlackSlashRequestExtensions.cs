using System.Text.RegularExpressions;
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

        public static User ToKickUser(this SlackSlashRequestDto slackSlashCommand, string userId)
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
            if (slackSlashCommand.text != null && slackSlashCommand.text.Contains(' ') && Regex.IsMatch(slackSlashCommand.text, "kick <@\\w*\\|\\w*>"))
            {
                return slackSlashCommand.text?.Split(' ')[0].ToLowerInvariant();
            }

            return slackSlashCommand.text;
        }
    }
}
