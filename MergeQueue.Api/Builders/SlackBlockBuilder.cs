using MergeQueue.Api.Dtos;
using MergeQueue.Api.Types;

namespace MergeQueue.Api.Builders
{
    public static class SlackBlockBuilder
    {
        public static SlackBlockDto CreateSection()
        {
            return new()
            {
                Type = SlackBlockType.Section
            };
        }

        public static SlackBlockDto WithText(this SlackBlockDto block, string text)
        {
            block.Text = new SlackBlockTextDto
            {
                Type = SlackTextType.Markdown,
                Text = text
            };
            return block;
        }

        public static SlackBlockDto WithAccessory(this SlackBlockDto block,
            string actionId,
            string type,
            string placeholder
        )
        {
            block.Accessory = new SlackBlockAccessoryDto
            {
                ActionId = actionId,
                Type = type,
                Placeholder = new SlackBlockTextDto
                {
                    Type = SlackTextType.PlainText,
                    Text = placeholder
                }
            };
            return block;
        }
    }
}
