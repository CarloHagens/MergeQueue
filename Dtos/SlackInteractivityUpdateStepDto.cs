using System.Collections.Generic;

namespace MergeQueue.Dtos
{
    public class SlackInteractivityUpdateStepDto
    {
        public string WorkFlowStepEditId { get; set; }
        public Dictionary<string, SlackInputValueDto> Inputs { get; set; }
    }
}
