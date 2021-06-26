using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackSlashResponseDto
    {
        [JsonPropertyName("response_type")]
        public string ResponseType { get; set; }
        public string Text { get; set; }
        public IEnumerable<SlackBlockDto> Blocks { get; set; }
    }
}
