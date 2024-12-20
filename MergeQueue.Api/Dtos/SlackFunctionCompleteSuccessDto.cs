namespace MergeQueue.Api.Dtos
{
    public class SlackFunctionCompleteSuccessDto
    {
        public string FunctionExecutionId { get; set; }
        public object Outputs { get; set; }
    }
}