using Microsoft.AspNetCore.Mvc;
using MergeQueue.Api.Repositories;
using MergeQueue.Api.Types;
using MergeQueue.Api.Dtos;
using MergeQueue.Api.Entities;
using MergeQueue.Api.Builders;

namespace MergeQueue.Api.Controllers
{
    [Route("[controller]")]
    public class EventsController : BaseController
    {
        public EventsController(IConfiguration configuration, IQueueRepository queueRepository, HttpClient httpClient)
            : base(configuration, queueRepository, httpClient)
        {
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SlackEventSubscriptionRequestDto request)
        {
            if (request.Type == SlackEventRequestTypes.UrlVerification)
            {
                return Ok(request.Challenge);
            }

            if (request.Type == SlackEventRequestTypes.EventCallback
                && request.Event.Type == SlackEventTypes.WorkflowStepExecute)
            {
                switch (request.Event.CallbackId)
                {
                    case Commands.Join:
                        await JoinQueue(request);
                        break;
                    case Commands.Leave:
                        await LeaveQueue(request);
                        break;
                    case Commands.Jump:
                        await JumpQueue(request);
                        break;
                }
            }

            return Ok();
        }

        private async Task JoinQueue(SlackEventSubscriptionRequestDto request)
        {
            var user = CreateUserFromEventInputs(request);
            var wasUserAdded = await QueueRepository.AddUser(user);

            if (!wasUserAdded)
            {
                var alreadyInQueueBody = CreateAlreadyInQueueBody(user);
                await PostToUrlWithBody(SlackApiEndpoints.SendEphemeralMessage, alreadyInQueueBody);
            }
            else
            {
                var queuedUsers = await QueueRepository.GetUsersForChannel(user.ChannelId);
                var joinQueueBody = CreateJoinQueueBody(user, queuedUsers);
                var showQueueBody = CreateShowQueueBody(user, queuedUsers);
                await PostToUrlWithBody(SlackApiEndpoints.SendMessage, joinQueueBody);
                await PostToUrlWithBody(SlackApiEndpoints.SendEphemeralMessage, showQueueBody);
            }
        }

        private async Task LeaveQueue(SlackEventSubscriptionRequestDto request)
        {
            var user = CreateUserFromEventInputs(request);
            var wasUserRemoved = await QueueRepository.RemoveUser(user);

            if (!wasUserRemoved)
            {
                var userNotInQueueBody = CreateUserNotInQueueBody(user);
                await PostToUrlWithBody(SlackApiEndpoints.SendEphemeralMessage, userNotInQueueBody);
            }
            else
            {
                var queuedUsers = await QueueRepository.GetUsersForChannel(user.ChannelId);
                var leaveQueueBody = CreateLeaveQueueBody(user, queuedUsers);
                await PostToUrlWithBody(SlackApiEndpoints.SendMessage, leaveQueueBody);
            }
        }

        private async Task JumpQueue(SlackEventSubscriptionRequestDto request)
        {
            var user = CreateUserFromEventInputs(request);
            await QueueRepository.Jump(user);

            var queuedUsers = await QueueRepository.GetUsersForChannel(user.ChannelId);
            var jumpedTheQueueBody = CreateJumpQueueBody(user, queuedUsers);
            await PostToUrlWithBody(SlackApiEndpoints.SendMessage, jumpedTheQueueBody);

            var showQueueBody = CreateShowQueueBody(user, queuedUsers);
            await PostToUrlWithBody(SlackApiEndpoints.SendEphemeralMessage, showQueueBody);
        }

        private static User CreateUserFromEventInputs(SlackEventSubscriptionRequestDto request)
        {
            var channelId = request.Event.WorkflowStep.Inputs[InputTypes.SelectedChannel].Value;
            var userId = request.Event.WorkflowStep.Inputs[InputTypes.SelectedUser].Value;

            return new User
            {
                ChannelId = channelId,
                UserId = userId
            };
        }

        private SlackSendMessageRequestDto CreateShowQueueBody(User user, List<User> queuedUsers)
        {
            var blockOfUsers = CreateShowQueueResponseBlocks(queuedUsers);

            return SlackSendMessageRequestBuilder
                .CreateSendMessageRequestToChannel(user.ChannelId)
                .ToUser(user.UserId)
                .WithBlocks(blockOfUsers);
        }

        private SlackSendMessageRequestDto CreateJoinQueueBody(User user, List<User> queuedUsers)
        {
            var responseText = CreateJoinQueueResponseText(user, queuedUsers);

            return SlackSendMessageRequestBuilder
                .CreateSendMessageRequestToChannel(user.ChannelId)
                .WithText(responseText);
        }

        private static SlackSendMessageRequestDto CreateAlreadyInQueueBody(User user) =>
            SlackSendMessageRequestBuilder
                .CreateSendMessageRequestToChannel(user.ChannelId)
                .ToUser(user.UserId)
                .WithText(ResponseMessages.AlreadyInQueue);

        private SlackSendMessageRequestDto CreateLeaveQueueBody(User user, List<User> queuedUsers)
        {
            var responseText = CreateLeaveQueueResponseText(user, queuedUsers);

            return SlackSendMessageRequestBuilder
                .CreateSendMessageRequestToChannel(user.ChannelId)
                .WithText(responseText);
        }

        private static SlackSendMessageRequestDto CreateUserNotInQueueBody(User user) =>
            SlackSendMessageRequestBuilder
                .CreateSendMessageRequestToChannel(user.ChannelId)
                .ToUser(user.UserId)
                .WithText(ResponseMessages.NotInQueue);

        private SlackSendMessageRequestDto CreateJumpQueueBody(User user, List<User> queuedUsers)
        {
            var responseText = CreateJumpQueueResponseText(user, queuedUsers);

            return SlackSendMessageRequestBuilder
                .CreateSendMessageRequestToChannel(user.ChannelId)
                .WithText(responseText);
        }
    }
}
