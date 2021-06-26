using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackInteractivityViewOpenDto
    {
        [JsonPropertyName("trigger_id")]
        public string TriggerId { get; set; }
        public SlackViewDto View { get; set; }
    }
}
