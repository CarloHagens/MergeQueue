using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackViewDto
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("submit_disabled")]
        public bool SubmitDisabled { get; set; }
        [JsonPropertyName("blocks")]
        public IEnumerable<SlackBlockDto> Blocks { get; set; }
    }
}