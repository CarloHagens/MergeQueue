using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MergeQueue.Repositories;
using MergeQueue.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MergeQueue.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        protected readonly IQueueRepository QueueRepository;

        public BaseController(IConfiguration configuration, IQueueRepository queueRepository, HttpClient httpClient)
        {
            QueueRepository = queueRepository;
            _httpClient = httpClient;
            if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                return;
            }
            var settings = configuration.GetSection(nameof(SlackApiSettings)).Get<SlackApiSettings>();
            _httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {settings.Token}");
        }

        protected async Task PostToUrlWithBody(string url, object body)
        {
            var serializationSettings = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = new SnakeCaseNamingPolicy()
            };
            var serializedBody = JsonSerializer.Serialize(body, serializationSettings);
            var content = new StringContent(serializedBody, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync(url, content);
        }
    }
}
