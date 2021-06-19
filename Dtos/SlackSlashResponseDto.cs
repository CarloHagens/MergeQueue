using System.Collections.Generic;
using Newtonsoft.Json;

namespace MergeQueue.Dtos
{
    public class SlackSlashResponseDto
    {
        [JsonProperty("response_type")]
        public string ResponseType { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("blocks")]
        public IEnumerable<SlackSlashResponseBlock> Blocks { get; set; }
    }
}
