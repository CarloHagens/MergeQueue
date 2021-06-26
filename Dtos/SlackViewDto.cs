using System.Collections.Generic;

namespace MergeQueue.Dtos
{
    public class SlackViewResponseDto
    {
        public string Type { get; set; }
        public IEnumerable<SlackBlockDto> Blocks { get; set; }
        public SlackViewStateDto State { get; set; }
    }
}