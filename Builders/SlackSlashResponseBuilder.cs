using MergeQueue.Dtos;
using MergeQueue.Types;

namespace MergeQueue.Builders
{
    public static class SlackSlashResponseBuilder
    {
        public static SlackBlockDto CreateSectionWithText(string text)
        {
            return new()
            {
                Type = SlackBlockType.Section,
                Text = new SlackBlockTextDto
                {
                    Type = SlackTextType.Markdown,
                    Text = text
                }
            };
        }

        public static SlackSlashResponseDto CreateEphemeralResponseWithText(string text)
        {
            return new()
            {
                ResponseType = SlackMessageType.Ephemeral,
                Text = text
            };
        }

        public static SlackSlashResponseDto CreateChannelResponseWithText(string text)
        {
            return new()
            {
                ResponseType = SlackMessageType.InChannel,
                Text = text
            };
        }
    }
}
