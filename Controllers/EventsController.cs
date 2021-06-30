using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MergeQueue.Dtos;
using MergeQueue.Entities;
using MergeQueue.Repositories;
using MergeQueue.Types;
using Microsoft.Extensions.Configuration;

namespace MergeQueue.Controllers
{
    [Route("[controller]")]
    public class EventsController : BaseController
    {
        public EventsController(IConfiguration configuration, IQueueRepository repository, HttpClient httpClient) 
            : base(configuration, repository, httpClient)
        {
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SlackEventSubscriptionRequestDto request)
        {
            if (request.Type == SlackEventTypes.UrlVerification)
            {
                return Ok(request.Challenge);
            }
            if (request.Type == SlackEventTypes.EventCallback 
                && request.Event.Type == "workflow_step_execute"
                && request.Event.CallbackId == Commands.Join)
            {
                var channelId = request.Event.WorkflowStep.Inputs["selected_channel"].Value;
                var userId = request.Event.WorkflowStep.Inputs["selected_user"].Value;
                var user = new User
                {
                    ChannelId = channelId,
                    UserId = userId
                };
                Repository.AddUser(user);

                var body = CreateJoinQueueBody(channelId, userId);
                await PostToUrlWithBody(SlackApiEndpoints.SendMessage, body);

                return Ok();
            }

            return Ok();
        }

        private SlackSendMessageRequestDto CreateJoinQueueBody(string channelId, string userId)
        {
            var queuedUsers = Repository.GetUsersForChannel(channelId);
            var responseText = queuedUsers.Count == 1
                ? $"<@{userId}> is now first in the queue!"
                : $"<@{userId}> joined the queue.";
            var body = new SlackSendMessageRequestDto
            {
                Channel = channelId,
                Text = responseText
            };
            return body;
        }
    }
}
