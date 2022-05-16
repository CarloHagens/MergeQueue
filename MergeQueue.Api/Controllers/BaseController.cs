using System.Text;
using System.Text.Json;
using MergeQueue.Api.Builders;
using MergeQueue.Api.Dtos;
using MergeQueue.Api.Entities;
using MergeQueue.Api.Filters;
using MergeQueue.Api.Repositories;
using MergeQueue.Api.Settings;
using MergeQueue.Api.Types;
using Microsoft.AspNetCore.Mvc;

namespace MergeQueue.Api.Controllers
{
    [ApiController]
    [TypeFilter(typeof(AuthenticationFilter))]
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
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.BotToken}");
        }

        protected static IEnumerable<SlackBlockDto> CreateShowQueueResponseBlocks(IEnumerable<User> queuedUsers)
        {
            return queuedUsers.Select((queuedUser, index) =>
                SlackBlockBuilder
                    .CreateSection()
                    .WithText(ResponseMessages.UserQueuePosition(index, queuedUser.UserId))
            ).ToList();
        }

        protected static string CreateJoinQueueResponseText(User user, IReadOnlyCollection<User> queuedUsers) =>
            queuedUsers.Count == 1
                ? ResponseMessages.UserFirstInQueue(user.UserId)
                : ResponseMessages.UserJoinedQueue(user.UserId);

        protected static string CreateLeaveQueueResponseText(User user, IReadOnlyCollection<User> queuedUsers)
        {
            var responseText = ResponseMessages.UserLeftQueue(user.UserId);
            return CreateRemoveQueueResponseText(queuedUsers, responseText);
        }

        protected static string CreateKickQueueResponseText(User user, IReadOnlyCollection<User> queuedUsers)
        {
            var responseText = ResponseMessages.UserKickedFromTheQueue(user.UserId);
            return CreateRemoveQueueResponseText(queuedUsers, responseText);
        }

        protected static string CreateJumpQueueResponseText(User user, IReadOnlyCollection<User> queuedUsers) =>
            queuedUsers.Count == 1
                ? ResponseMessages.UserFirstInQueue(user.UserId)
                : ResponseMessages.UserJumpedQueue(user.UserId);

        protected async Task PostToUrlWithBody(string url, object body)
        {
            var serializationSettings = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = new SnakeCaseNamingPolicy()
            };
            var serializedBody = JsonSerializer.Serialize(body, serializationSettings);
            var content = new StringContent(serializedBody, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync(url, content);
        }

        private static string CreateRemoveQueueResponseText(IReadOnlyCollection<User> queuedUsers, string responseText)
        {
            if (queuedUsers.Count > 0)
            {
                responseText += ResponseMessages.UserTurnArrived(queuedUsers.First().UserId);
            }
            else
            {
                responseText += ResponseMessages.QueueNowEmpty;
            }

            return responseText;
        }
    }
}
