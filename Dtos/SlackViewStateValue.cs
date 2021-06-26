using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackViewStateValue
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
        [JsonPropertyName("selected_user")]
        public string SelectedUser { get; set; }
        [JsonPropertyName("selected_channel")]
        public string SelectedChannel { get; set; }
    }
}
