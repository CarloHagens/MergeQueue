using MergeQueue.Dtos;
using MergeQueue.Types;

namespace MergeQueue.Builders
{
    public static class SlackBlockBuilder
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
    }
}
