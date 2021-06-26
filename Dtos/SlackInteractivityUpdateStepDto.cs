using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackInteractivityUpdateStepDto
    {
        [JsonPropertyName("workflow_step_edit_id")]
        public string WorkFlowStepEditId { get; set; }
        public Dictionary<string, SlackInputValueDto> Inputs {get; set; }
    }
}
