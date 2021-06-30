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
        protected readonly IQueueRepository Repository;
        protected readonly HttpClient HttpClient;

        public BaseController(IQueueRepository repository, HttpClient httpClient)
        {
            Repository = repository;
            HttpClient = httpClient;
            if (!HttpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                HttpClient.DefaultRequestHeaders.Add("Authorization",
                    "Bearer xoxb-2174785981490-2171724521813-wsp1iua7epk4dI3IQDUhEaft");
            }
        }

        protected async Task PostToUrlWithBody(string url, object body)
        {
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            await HttpClient.PostAsync(url, content);
        }
    }
}
