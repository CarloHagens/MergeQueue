using Microsoft.AspNetCore.Mvc;
using MergeQueue.Api.Builders;
using MergeQueue.Api.Extensions;
using MergeQueue.Api.Dtos;
using MergeQueue.Api.Repositories;
using MergeQueue.Api.Entities;
using MergeQueue.Api.Types;

namespace MergeQueue.Api.Controllers
{
    [Route("[controller]")]
    public class SlashCommandsController : BaseController
    {
        public SlashCommandsController(IConfiguration configuration, IQueueRepository queueRepository, HttpClient httpClient)
            : base(configuration, queueRepository, httpClient)
        {
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] SlackSlashRequestDto request)
        {
            SlackSlashResponseDto? responseBody;
            if (request.GetCommand() == Commands.Show)
            {
                responseBody = await ShowQueue(request);
            }
            else if (request.GetCommand() == Commands.Join)
            {
                responseBody = await JoinQueue(request);
            }
            else if (request.GetCommand() == Commands.Leave)
            {
                responseBody = await LeaveQueue(request);
            }
            else if (request.GetCommand() == Commands.Jump)
            {
                responseBody = await JumpQueue(request);
            }

            else if (request.GetKickCommand() == Commands.Kick)
            {
                responseBody = await KickFromQueue(request);
            }
            else
            {
                responseBody = Help();
            }

            if (responseBody.Exists())
            {
                return Ok(responseBody);
            }

            return Ok();
        }

        private async Task<SlackSlashResponseDto> ShowQueue(SlackSlashRequestDto request)
        {
            var queuedUsers = await QueueRepository.GetUsersForChannel(request.channel_id);

            SlackSlashResponseDto responseBody;
            if (queuedUsers.Any())
            {
                responseBody = CreateShowQueueBody(queuedUsers);
            }
            else
            {
                responseBody = SlackSlashResponseBuilder
                    .CreateEphemeralResponse()
                    .WithText(ResponseMessages.QueueIsEmpty);
            }

            return responseBody;
        }

        private async Task<SlackSlashResponseDto> JoinQueue(SlackSlashRequestDto request)
        {
            var user = request.ToUser();
            var wasUserAdded = await QueueRepository.AddUser(user);

            SlackSlashResponseDto responseBody;
            if (!wasUserAdded)
            {
                responseBody = SlackSlashResponseBuilder
                    .CreateEphemeralResponse()
                    .WithText(ResponseMessages.AlreadyInQueue);
            }
            else
            {
                var users = await QueueRepository.GetUsersForChannel(user.ChannelId);
                var joinQueueBody = CreateJoinQueueBody(user, users);
                await PostToUrlWithBody(request.response_url, joinQueueBody);
                responseBody = CreateShowQueueBody(users);
            }

            return responseBody;
        }

        private async Task<SlackSlashResponseDto> JumpQueue(SlackSlashRequestDto request)
        {
            var user = request.ToUser();
            await QueueRepository.Jump(user);

            var users = await QueueRepository.GetUsersForChannel(user.ChannelId);
            var body = CreateJumpQueueBody(user, users);
            await PostToUrlWithBody(request.response_url, body);
            return CreateShowQueueBody(users);
        }

        private async Task<SlackSlashResponseDto?> LeaveQueue(SlackSlashRequestDto request)
        {
            var user = request.ToUser();
            var wasUserRemoved = await QueueRepository.RemoveUser(user);

            SlackSlashResponseDto? responseBody = null;
            if (!wasUserRemoved)
            {
                responseBody = SlackSlashResponseBuilder
                    .CreateEphemeralResponse()
                    .WithText(ResponseMessages.NotInQueue);
            }
            else
            {
                var users = await QueueRepository.GetUsersForChannel(user.ChannelId);
                var leaveQueueBody = CreateLeaveQueueBody(user, users);
                await PostToUrlWithBody(request.response_url, leaveQueueBody);
            }

            return responseBody;
        }

        private async Task<SlackSlashResponseDto?> KickFromQueue(SlackSlashRequestDto request)
        {
            var userIdToKick = request.text.Split('@')[1].Trim();
            if (userIdToKick.Contains('|'))
            {
                userIdToKick = userIdToKick.Split('|')[0].Trim();
            }
            else
            {
                userIdToKick = userIdToKick.Split('>')[0].Trim();
            }
            var user = request.ToUserToKick(userIdToKick);
            var wasPersonRemoved = await QueueRepository.RemoveUser(user);

            SlackSlashResponseDto? responseBody = null;
            if (!wasPersonRemoved)
            {
                responseBody = SlackSlashResponseBuilder
                    .CreateEphemeralResponse()
                    .WithText(ResponseMessages.UserNotInQueue(userIdToKick));
            }
            else
            {
                var users = await QueueRepository.GetUsersForChannel(user.ChannelId);
                var body = CreateKickQueueBody(user, users);
                await PostToUrlWithBody(request.response_url, body);
            }

            return responseBody;
        }

        private static SlackSlashResponseDto Help() =>
            SlackSlashResponseBuilder
                .CreateEphemeralResponse()
                .WithBlocks(new List<SlackBlockDto>
                {
                    SlackBlockBuilder
                        .CreateSection()
                        .WithText(ResponseMessages.QueueIntroduction),
                    SlackBlockBuilder
                        .CreateSection()
                        .WithText(ResponseMessages.AvailableCommands),
                    SlackBlockBuilder
                        .CreateSection()
                        .WithText(ResponseMessages.QueueCommands)
                });

        private SlackSlashResponseDto CreateShowQueueBody(List<User> users)
        {
            var blockOfUsers = CreateShowQueueResponseBlocks(users);

            return SlackSlashResponseBuilder
                .CreateEphemeralResponse()
                .WithBlocks(blockOfUsers);
        }

        private SlackSlashResponseDto CreateJoinQueueBody(User user, List<User> users)
        {
            var responseText = users.Count == 1
                ? ResponseMessages.UserFirstInQueue(user.UserId)
                : ResponseMessages.UserJoinedQueue(user.UserId);

            return SlackSlashResponseBuilder
                .CreateChannelResponse()
                .WithText(responseText);
        }

        private SlackSlashResponseDto CreateJumpQueueBody(User user, List<User> users)
        {
            var responseText = CreateJumpQueueResponseText(user, users);
            return SlackSlashResponseBuilder
                .CreateChannelResponse()
                .WithText(responseText);
        }

        private SlackSlashResponseDto CreateLeaveQueueBody(User user, List<User> users)
        {
            var responseText = CreateLeaveQueueResponseText(user, users);
            return SlackSlashResponseBuilder
                .CreateChannelResponse()
                .WithText(responseText);
        }

        private SlackSlashResponseDto CreateKickQueueBody(User user, List<User> users)
        {
            var responseText = CreateKickQueueResponseText(user, users);

            return SlackSlashResponseBuilder
                .CreateChannelResponse()
                .WithText(responseText);
        }
    }
}
