using System.Collections.Generic;

namespace MergeQueue.Api.Dtos
{
    public class SlackSlashResponseDto
    {
        public string ResponseType { get; set; }
        public string Text { get; set; }
        public IEnumerable<SlackBlockDto> Blocks { get; set; }
    }
}
