namespace MergeQueue.Api.Dtos
{
    public class SlackEventDto
    {
        public string Type { get; set; }
        public string FunctionExecutionId { get; set; }
        public EventFunctionDto Function { get; set; }
        public EventInputDto Inputs { get; set; }
    }
}