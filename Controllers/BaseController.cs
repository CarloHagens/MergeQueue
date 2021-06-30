using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MergeQueue.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MergeQueue.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        protected readonly IQueueRepository Repository;

        public BaseController(IQueueRepository repository, HttpClient httpClient)
        {
            Repository = repository;
            _httpClient = httpClient;
            if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization",
                    "Bearer xoxb-2174785981490-2171724521813-wsp1iua7epk4dI3IQDUhEaft");
            }
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
