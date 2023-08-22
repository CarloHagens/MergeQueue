using System.Text.Json;
using MergeQueue.Api.Dtos;
using System.Text.Json.Serialization;

namespace MergeQueue.Api.Services
{
    public class SlackService : ISlackService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger _logger;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public SlackService(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            jsonSerializerOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = new SnakeCaseNamingPolicy()
            };
            _logger = logger;
        }

        public async Task OpenView(SlackInteractivityViewOpenDto body)
        {
            await httpClient.PostAsJsonAsync(SlackServiceEndpoints.OpenView, body, jsonSerializerOptions);
        }

        public async Task UpdateWorkflowStep(SlackInteractivityUpdateStepDto body)
        {
            await httpClient.PostAsJsonAsync(SlackServiceEndpoints.UpdateWorkflowStep, body, jsonSerializerOptions);
        }

        public async Task SendMessage(SlackSendMessageRequestDto body)
        {
            _logger.LogInformation("Sending request to slack service");
            _logger.LogInformation($"request url: {SlackServiceEndpoints.SendMessage}");
            _logger.LogInformation($"request headers: {httpClient.DefaultRequestHeaders}");
            _logger.LogInformation($"request body: {body}");
            var response = await httpClient.PostAsJsonAsync(SlackServiceEndpoints.SendMessage, body, jsonSerializerOptions);
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"response status code: {response.StatusCode}");
            _logger.LogInformation($"response headers: {response.Headers}");
            _logger.LogInformation($"response body: {responseBody}");
            _logger.LogInformation("Successfully sent request to slack");
        }

        public async Task SendEphemeralMessage(SlackSendMessageRequestDto body)
        {
            _logger.LogInformation("Sending request to slack service");
            _logger.LogInformation($"request url: {SlackServiceEndpoints.SendEphemeralMessage}");
            _logger.LogInformation($"request headers: {httpClient.DefaultRequestHeaders}");
            _logger.LogInformation($"request body: {body}");
            var response = await httpClient.PostAsJsonAsync(SlackServiceEndpoints.SendEphemeralMessage, body, jsonSerializerOptions);
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"response status code: {response.StatusCode}");
            _logger.LogInformation($"response headers: {response.Headers}");
            _logger.LogInformation($"response body: {responseBody}");
            _logger.LogInformation("Successfully sent request to slack");
        }

        public async Task SendResponse(string url, SlackSlashResponseDto body)
        {
            await httpClient.PostAsJsonAsync(url, body, jsonSerializerOptions);
        }
    }
}
