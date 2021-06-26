using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackViewStateValue
    {
        public string Type { get; set; }
        public string Value { get; set; }
        [JsonPropertyName("selected_user")]
        public string SelectedUser { get; set; }
        [JsonPropertyName("selected_channel")]
        public string SelectedChannel { get; set; }
    }
}
