using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackBlockDto
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("text")]
        public SlackBlockTextDto Text { get; set; }
        [JsonPropertyName("accessory")]
        public SlackBlockAccessoryDto Accessory { get; set; }
    }
}