using System.Collections.Generic;

namespace MergeQueue.Api.Dtos
{
    public class EventWorkflowStepDto
    {
        public Dictionary<string, SlackInputValueDto> Inputs { get; set; }
    }
}