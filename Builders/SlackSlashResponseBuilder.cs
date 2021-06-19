using MergeQueue.Dtos;

namespace MergeQueue.Builders
{
    public static class SlackSlashResponseBuilder
    {
        public static SlackSlashResponseBlock CreateSectionWithText(string text)
        {
            return new()
            {
                Type = SlackSlashResponseBlockType.Section,
                Text = new SlackSlashResponseText
                {
                    Type = SlackSlashResponseTextType.Markdown,
                    Text = text
                }
            };
        }

        public static SlackSlashResponseDto CreateEphemeralResponseWithText(string text)
        {
            return new()
            {
                ResponseType = SlackSlashResponseType.Ephemeral,
                Text = text
            };
        }

        public static SlackSlashResponseDto CreateChannelResponseWithText(string text)
        {
            return new()
            {
                ResponseType = SlackSlashResponseType.InChannel,
                Text = text
            };
        }
    }
}
