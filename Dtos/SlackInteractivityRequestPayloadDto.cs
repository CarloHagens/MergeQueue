namespace MergeQueue.Dtos
{
    public class SlackInteractivityRequestPayloadDto
    {
        public string Type { get; set; }
        public string Token { get; set; }
        public string ActionTs { get; set; }
        public string CallbackId { get; set; }
        public string TriggerId { get; set; }
        public SlackViewResponseDto View { get; set; }
        public WorkflowStepDto WorkflowStep { get; set; }
    }
}
