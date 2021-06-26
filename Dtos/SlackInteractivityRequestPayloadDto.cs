using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackInteractivityRequestPayloadDto
    {
        public string Type { get; set; }
        public string Token { get; set; }
        [JsonPropertyName("action_ts")]
        public string ActionTs { get; set; }
        [JsonPropertyName("callback_id")]
        public string CallbackId { get; set; }
        [JsonPropertyName("trigger_id")]
        public string TriggerId { get; set; }
        public SlackViewResponseDto View { get; set; }
        [JsonPropertyName("workflow_step")]
        public WorkflowStepDto WorkflowStep { get; set; }
    }
}
