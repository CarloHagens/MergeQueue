using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class WorkflowStepDto
    {
        [JsonPropertyName("workflow_step_edit_id")]
        public string WorkflowStepEditId { get; set; }
        [JsonPropertyName("workflow_id")]
        public string WorkflowId { get; set; }
        [JsonPropertyName("step_id")]
        public string StepId { get; set; }
    }
}
