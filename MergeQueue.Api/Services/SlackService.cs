using System.Text.Json;
using MergeQueue.Api.Dtos;
using System.Text.Json.Serialization;

namespace MergeQueue.Api.Services
{
    public class SlackService : ISlackService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public SlackService(IHttpClientFactory httpClientFactory) {
            this.httpClientFactory = httpClientFactory;
            jsonSerializerOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = new SnakeCaseNamingPolicy()
            };
        }

        public async Task OpenView(SlackInteractivityViewOpenDto body)
        {
            await httpClientFactory.CreateClient().PostAsJsonAsync(SlackServiceEndpoints.OpenView, body, jsonSerializerOptions);
        }

        public async Task UpdateWorkflowStep(SlackInteractivityUpdateStepDto body)
        {
            await httpClientFactory.CreateClient().PostAsJsonAsync(SlackServiceEndpoints.UpdateWorkflowStep, body, jsonSerializerOptions);
        }

        public async Task SendMessage(SlackSendMessageRequestDto body)
        {
            await httpClientFactory.CreateClient().PostAsJsonAsync(SlackServiceEndpoints.SendMessage, body, jsonSerializerOptions);
        }

        public async Task SendEphemeralMessage(SlackSendMessageRequestDto body)
        {
            await httpClientFactory.CreateClient().PostAsJsonAsync(SlackServiceEndpoints.SendEphemeralMessage, body, jsonSerializerOptions);
        }

        public async Task SendResponse(string url, SlackSlashResponseDto body)
        {
            await httpClientFactory.CreateClient().PostAsJsonAsync(url, body, jsonSerializerOptions);
        }
    }
}
