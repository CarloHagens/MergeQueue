using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackInteractivityRequestPayloadDto
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonPropertyName("action_ts")]
        public string ActionTs { get; set; }
        [JsonPropertyName("callback_id")]
        public string CallbackId { get; set; }
        [JsonPropertyName("trigger_id")]
        public string TriggerId { get; set; }
        [JsonPropertyName("view")]
        public SlackViewResponseDto View { get; set; }
        [JsonPropertyName("workflow_step")]
        public WorkflowStepDto WorkflowStep { get; set; }
    }
}
