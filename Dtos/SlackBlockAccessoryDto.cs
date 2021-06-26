using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackBlockAccessoryDto
    {
        [JsonPropertyName("action_id")]
        public string ActionId { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("placeholder")]
        public SlackBlockTextDto Placeholder { get; set; }
    }
}