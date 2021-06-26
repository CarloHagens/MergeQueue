using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackBlockAccessoryDto
    {
        [JsonPropertyName("action_id")]
        public string ActionId { get; set; }
        public string Type { get; set; }
        public SlackBlockTextDto Placeholder { get; set; }
    }
}