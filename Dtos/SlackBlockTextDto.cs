using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackBlockTextDto
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}