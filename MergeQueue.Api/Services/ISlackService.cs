using MergeQueue.Api.Dtos;

namespace MergeQueue.Api.Services
{
    public interface ISlackService
    {
        Task OpenView(SlackInteractivityViewOpenDto body);
        Task UpdateWorkflowStep(SlackInteractivityUpdateStepDto body);
        Task SendMessage(SlackSendMessageRequestDto body);
        Task SendEphemeralMessage(SlackSendMessageRequestDto body);
        Task SendResponse(string url, SlackSlashResponseDto body);
    }
}
