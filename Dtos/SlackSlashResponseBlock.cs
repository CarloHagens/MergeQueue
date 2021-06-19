using Newtonsoft.Json;

namespace MergeQueue.Dtos
{
    public class SlackSlashResponseBlock
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("text")]
        public SlackSlashResponseText Text { get; set; }
    }
}