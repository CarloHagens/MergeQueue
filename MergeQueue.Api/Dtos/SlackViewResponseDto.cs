using System.Collections.Generic;

namespace MergeQueue.Api.Dtos
{
    public class SlackViewDto
    {
        public string Type { get; set; }
        public bool SubmitDisabled { get; set; }
        public IEnumerable<SlackBlockDto> Blocks { get; set; }
    }
}