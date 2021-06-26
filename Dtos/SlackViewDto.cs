using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackViewResponseDto
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("blocks")]
        public IEnumerable<SlackBlockDto> Blocks { get; set; }
        [JsonPropertyName("state")]
        public SlackViewStateDto State { get; set; }
    }
}