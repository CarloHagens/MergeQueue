using System.Text.Json;
using MergeQueue.Api.Dtos;
using System.Text.Json.Serialization;
using MergeQueue.Api.Types;

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
            await httpClientFactory.CreateClient().PostAsJsonAsync(SlackApiEndpoints.OpenView, body, jsonSerializerOptions);
        }

        public async Task UpdateWorkflowStep(SlackInteractivityUpdateStepDto body)
        {
            await httpClientFactory.CreateClient().PostAsJsonAsync(SlackApiEndpoints.UpdateWorkflowStep, body, jsonSerializerOptions);
        }

        public async Task SendMessage(SlackSendMessageRequestDto body)
        {
            await httpClientFactory.CreateClient().PostAsJsonAsync(SlackApiEndpoints.SendMessage, body, jsonSerializerOptions);
        }

        public async Task SendEphemeralMessage(SlackSendMessageRequestDto body)
        {
            await httpClientFactory.CreateClient().PostAsJsonAsync(SlackApiEndpoints.SendEphemeralMessage, body, jsonSerializerOptions);
        }

        public async Task SendResponse(string url, SlackSlashResponseDto body)
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("");
            await httpClientFactory.CreateClient().PostAsJsonAsync(url, body, jsonSerializerOptions);
        }
    }
}
