using MergeQueue.Api.Dtos;

namespace MergeQueue.Api.Services
{
    public interface ISlackService
    {
        Task FunctionCompleteSuccess(SlackFunctionCompleteSuccessDto body);
        Task SendMessage(SlackSendMessageRequestDto body);
        Task SendEphemeralMessage(SlackSendMessageRequestDto body);
        Task SendResponse(string url, SlackSlashResponseDto body);
    }
}
