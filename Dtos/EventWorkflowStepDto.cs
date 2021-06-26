using System.Collections.Generic;

namespace MergeQueue.Dtos
{
    public class EventWorkflowStepDto
    {
        public Dictionary<string, SlackInputValueDto> Inputs { get; set; }
    }
}