namespace MergeQueue.Dtos
{
    public class SlackEventDto
    {
        public string Type { get; set; }
        public string CallbackId { get; set; }
        public EventWorkflowStepDto WorkflowStep { get; set; }
    }
}