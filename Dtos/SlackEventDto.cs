using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackEventDto
    {
        public string Type { get; set; }
        [JsonPropertyName("callback_id")]
        public string CallbackId { get; set; }
        [JsonPropertyName("workflow_step")]
        public EventWorkflowStepDto WorkflowStep { get; set; }
    }
}