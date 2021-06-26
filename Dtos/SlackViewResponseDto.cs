using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackViewDto
    {
        public string Type { get; set; }
        [JsonPropertyName("submit_disabled")]
        public bool SubmitDisabled { get; set; }
        public IEnumerable<SlackBlockDto> Blocks { get; set; }
    }
}