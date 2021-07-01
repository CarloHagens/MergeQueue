using MergeQueue.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MergeQueue.Builders;
using MergeQueue.Dtos;
using MergeQueue.Entities;
using MergeQueue.Extensions;
using MergeQueue.Types;
using Microsoft.Extensions.Configuration;

namespace MergeQueue.Controllers
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
            SlackSlashResponseDto responseBody;
            if (request.GetCommand() == Commands.Show)
            {
                responseBody = ShowQueue(request);
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

        private SlackSlashResponseDto ShowQueue(SlackSlashRequestDto request)
        {
            var queuedUsers = QueueRepository.GetUsersForChannel(request.channel_id);

            SlackSlashResponseDto responseBody;
            if (queuedUsers.Any())
            {
                responseBody = CreateShowQueueBody(null, queuedUsers);
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
            var wasUserAdded = QueueRepository.AddUser(user);

            SlackSlashResponseDto responseBody;
            if (!wasUserAdded)
            {
                responseBody = SlackSlashResponseBuilder
                    .CreateEphemeralResponse()
                    .WithText(ResponseMessages.AlreadyInQueue);
            }
            else
            {
                var joinQueueBody = CreateJoinQueueBody(user);
                await PostToUrlWithBody(request.response_url, joinQueueBody);
                responseBody = CreateShowQueueBody(user.ChannelId);
            }

            return responseBody;
        }

        private async Task<SlackSlashResponseDto> JumpQueue(SlackSlashRequestDto request)
        {
            var user = request.ToUser();
            QueueRepository.Jump(user);
            var body = CreateJumpQueueBody(user);
            await PostToUrlWithBody(request.response_url, body);
            return CreateShowQueueBody(user.ChannelId);
        }

        private async Task<SlackSlashResponseDto> LeaveQueue(SlackSlashRequestDto request)
        {
            var user = request.ToUser();
            var leaveQueueBody = CreateLeaveQueueBody(user);
            var wasUserRemoved = QueueRepository.RemoveUser(user);

            SlackSlashResponseDto responseBody = null;
            if (!wasUserRemoved)
            {
                responseBody = SlackSlashResponseBuilder
                    .CreateEphemeralResponse()
                    .WithText(ResponseMessages.NotInQueue);
            }
            else
            {
                await PostToUrlWithBody(request.response_url, leaveQueueBody);
            }

            return responseBody;
        }

        private async Task<SlackSlashResponseDto> KickFromQueue(SlackSlashRequestDto request)
        {
            var userIdToKick = request.text.Split('@')[1].Split('|')[0].Trim();
            var user = request.ToUserToKick(userIdToKick);
            var body = CreateKickQueueBody(user);
            var wasPersonRemoved = QueueRepository.RemoveUser(user);

            SlackSlashResponseDto responseBody = null;
            if (!wasPersonRemoved)
            {
                responseBody = SlackSlashResponseBuilder
                    .CreateEphemeralResponse()
                    .WithText(ResponseMessages.UserNotInQueue(userIdToKick));
            }
            else
            {
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

        private SlackSlashResponseDto CreateShowQueueBody(string channelId = null, List<User> queuedUsers = null)
        {
            if (queuedUsers.DoesNotExist() && channelId.Exists())
            {
                queuedUsers = QueueRepository.GetUsersForChannel(channelId);
            }

            var blockOfUsers = CreateShowQueueResponseBlocks(queuedUsers);

            return SlackSlashResponseBuilder
                .CreateEphemeralResponse()
                .WithBlocks(blockOfUsers);
        }

        private SlackSlashResponseDto CreateJoinQueueBody(User user)
        {
            var queuedUsers = QueueRepository.GetUsersForChannel(user.ChannelId);
            var responseText = queuedUsers.Count == 1
                ? ResponseMessages.UserFirstInQueue(user.UserId)
                : ResponseMessages.UserJoinedQueue(user.UserId);

            return SlackSlashResponseBuilder
                .CreateChannelResponse()
                .WithText(responseText);
        }

        private static SlackSlashResponseDto CreateJumpQueueBody(User user)
        {
            var responseText = ResponseMessages.UserJumpedQueue(user.UserId);
            return SlackSlashResponseBuilder
                .CreateChannelResponse()
                .WithText(responseText);
        }

        private SlackSlashResponseDto CreateLeaveQueueBody(User user)
        {
            var queuedUsers = QueueRepository.GetUsersForChannel(user.ChannelId);
            var responseText = CreateLeaveQueueResponseText(user, queuedUsers);
            return SlackSlashResponseBuilder
                .CreateChannelResponse()
                .WithText(responseText);

        }

        private SlackSlashResponseDto CreateKickQueueBody(User user)
        {
            var queuedUsers = QueueRepository.GetUsersForChannel(user.ChannelId);
            var responseText = CreateKickQueueResponseText(user, queuedUsers);

            return SlackSlashResponseBuilder
                .CreateChannelResponse()
                .WithText(responseText);
        }
    }
}
