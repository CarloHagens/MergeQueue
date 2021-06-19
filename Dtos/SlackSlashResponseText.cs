using Newtonsoft.Json;

namespace MergeQueue.Dtos
{
    public class SlackSlashResponseText
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}