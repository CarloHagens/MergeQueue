﻿using Microsoft.AspNetCore.Mvc;
using MergeQueue.Api.Repositories;
using MergeQueue.Api.Types;
using MergeQueue.Api.Dtos;
using MergeQueue.Api.Entities;
using MergeQueue.Api.Builders;
using MergeQueue.Api.Services;

namespace MergeQueue.Api.Controllers
{
    [Route("[controller]")]
    public class EventsController : BaseController
    {
        private readonly IQueueLookup queueLookup;
        private readonly ISlackService slackService;

        public EventsController(IQueueLookup queueLookup, ISlackService slackService)
        {
            this.queueLookup = queueLookup;
            this.slackService = slackService;
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
            var wasUserAdded = await queueLookup.AddUser(user);

            if (!wasUserAdded)
            {
                var alreadyInQueueBody = CreateAlreadyInQueueBody(user);
                await slackService.SendEphemeralMessage(alreadyInQueueBody);
            }
            else
            {
                var queuedUsers = await queueLookup.GetUsersForChannel(user.ChannelId);
                var joinQueueBody = CreateJoinQueueBody(user, queuedUsers);
                var showQueueBody = CreateShowQueueBody(user, queuedUsers);
                await slackService.SendMessage(joinQueueBody);
                await slackService.SendEphemeralMessage(showQueueBody);
            }
        }

        private async Task LeaveQueue(SlackEventSubscriptionRequestDto request)
        {
            var user = CreateUserFromEventInputs(request);
            var queuedUsers = await queueLookup.GetUsersForChannel(user.ChannelId);
            var wasUserRemoved = await queueLookup.RemoveUser(user);

            if (!wasUserRemoved)
            {
                var userNotInQueueBody = CreateUserNotInQueueBody(user);
                await slackService.SendEphemeralMessage(userNotInQueueBody);
            }
            else
            {
                var leaveQueueBody = CreateLeaveQueueBody(user, queuedUsers);
                await slackService.SendMessage(leaveQueueBody);
            }
        }

        private async Task JumpQueue(SlackEventSubscriptionRequestDto request)
        {
            var user = CreateUserFromEventInputs(request);
            var wasUserAdded = await queueLookup.AddUser(user, 1);

            if (!wasUserAdded)
            {
                var alreadyInQueueBody = CreateAlreadyInQueueAtThatPositioonBody(user);
                await slackService.SendEphemeralMessage(alreadyInQueueBody);
            }
            else
            {

                var queuedUsers = await queueLookup.GetUsersForChannel(user.ChannelId);
                var jumpedTheQueueBody = CreateJumpQueueBody(user, queuedUsers);
                await slackService.SendMessage(jumpedTheQueueBody);

                var showQueueBody = CreateShowQueueBody(user, queuedUsers);
                await slackService.SendEphemeralMessage(showQueueBody);
            }
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

        private static SlackSendMessageRequestDto CreateAlreadyInQueueAtThatPositioonBody(User user) =>
            SlackSendMessageRequestBuilder
                .CreateSendMessageRequestToChannel(user.ChannelId)
                .ToUser(user.UserId)
                .WithText(ResponseMessages.AlreadyInQueueAtThatPosition);

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
