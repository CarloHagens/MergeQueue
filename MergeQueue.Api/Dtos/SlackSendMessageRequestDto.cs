using System.Collections.Generic;

namespace MergeQueue.Api.Dtos
{
    public class SlackSendMessageRequestDto
    {
        public string Channel { get; set; }
        public string Text { get; set; }
        public IEnumerable<SlackBlockDto> Blocks { get; set; }
        public string User { get; set; }
    }
}
