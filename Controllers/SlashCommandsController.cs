﻿using MergeQueue.Repositories;
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
        private readonly string _queueCommands = "" +
                                             $"`/queue {Commands.Show}` - Show a queue for this channel\n" +
                                             $"`/queue {Commands.Join}` - Join the queue\n" +
                                             $"`/queue {Commands.Leave}` - Leave the queue\n" +
                                             $"`/queue {Commands.Jump}` - Jump to the first position in the queue\n" +
                                             $"`/queue {Commands.Kick} @user` - Kick user from the queue\n" +
                                             $"`/queue {Commands.Help}` - Show all available commands";

        public SlashCommandsController(IConfiguration configuration, IQueueRepository queueRepository, HttpClient httpClient) 
            : base(configuration, queueRepository, httpClient)
        {
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] SlackSlashRequestDto request)
        {
            if (request.GetCommand() == Commands.Show)
            {
                var queuedPeople = QueueRepository.GetUsersForChannel(request.channel_id);
                if (queuedPeople.Count == 0)
                {
                    return Ok(SlackSlashResponseBuilder.CreateEphemeralResponseWithText("The queue is currently empty."));
                }
                var body = CreateShowQueueBody(queuedPeople);
                await PostToUrlWithBody(request.response_url, body);
                return Ok();
            }

            if (request.GetCommand() == Commands.Join)
            {
                var wasUserAdded = QueueRepository.AddUser(request.ToUser());
                if (!wasUserAdded)
                {
                    return Ok(SlackSlashResponseBuilder.CreateEphemeralResponseWithText("You are already in the queue."));
                }
                var body = CreateJoinQueueBody(request);
                await PostToUrlWithBody(request.response_url, body);
                return Ok();
            }

            if (request.GetCommand() == Commands.Leave)
            {
                var body = CreateLeaveQueueBody(request);

                var wasPersonRemoved = QueueRepository.RemoveUser(request.ToUser());
                if (!wasPersonRemoved)
                {
                    return Ok(SlackSlashResponseBuilder.CreateEphemeralResponseWithText("You are not in the queue."));
                }

                await PostToUrlWithBody(request.response_url, body);
                return Ok();
            }

            if (request.GetCommand() == Commands.Jump)
            {
                QueueRepository.Jump(request.ToUser());
                var body = CreateJumpQueueBody(request);
                await PostToUrlWithBody(request.response_url, body);
                return Ok();
            }

            if (request.GetKickCommand() == Commands.Kick)
            {
                var userIdToKick = request.text.Split('@')[1].Split('|')[0].Trim();
                var body = CreateLeaveQueueBody(request, userIdToKick);

                var wasPersonRemoved = QueueRepository.RemoveUser(request.ToKickUser(userIdToKick));
                if (!wasPersonRemoved)
                {
                    return Ok(SlackSlashResponseBuilder.CreateEphemeralResponseWithText($"<@{userIdToKick}> is not in the queue."));
                }

                await PostToUrlWithBody(request.response_url, body);
                return Ok();
            }

            var helpResponseBody = new SlackSlashResponseDto
            {
                ResponseType = SlackMessageType.Ephemeral,
                Blocks = new List<SlackBlockDto>
                    {
                        SlackBlockBuilder.CreateSectionWithText(":wave: Need some help with `/queue`?"),
                        SlackBlockBuilder.CreateSectionWithText("*Available commands*"),
                        SlackBlockBuilder.CreateSectionWithText(_queueCommands)
                    }
            };
            return Ok(helpResponseBody);
        }

        private SlackSlashResponseDto CreateLeaveQueueBody(SlackSlashRequestDto request, string userId = null)
        {
            var userIdToUse = userId ?? request.user_id;
            var responseText = $"<@{userIdToUse}> left the queue.";
            var queuedPeople = QueueRepository.GetUsersForChannel(request.channel_id);
            if (queuedPeople.Count > 0 && queuedPeople.First().UserId == userIdToUse)
            {
                if (queuedPeople.Count > 1)
                {
                    responseText += $"\n<@{queuedPeople[1].UserId}> it is now your turn!";
                }
                else
                {
                    responseText += " It is now empty.";
                }
            }

            var response = SlackSlashResponseBuilder.CreateChannelResponseWithText(responseText);
            return response;
        }

        private SlackSlashResponseDto CreateJoinQueueBody(SlackSlashRequestDto request)
        {
            var queuedUsers = QueueRepository.GetUsersForChannel(request.channel_id);
            var responseText = queuedUsers.Count == 1
                ? $"<@{request.user_id}> is now first in the queue!"
                : $"<@{request.user_id}> joined the queue.";
            var response = SlackSlashResponseBuilder.CreateChannelResponseWithText(responseText);
            return response;
        }

        private SlackSlashResponseDto CreateJumpQueueBody(SlackSlashRequestDto request)
        {
            var responseText = $"<@{request.user_id}> is now first in the queue!";
            var response = SlackSlashResponseBuilder.CreateChannelResponseWithText(responseText);
            return response;
        }

        private SlackSlashResponseDto CreateShowQueueBody(IEnumerable<User> queuedPeople)
        {
            var queue = queuedPeople.Select((queuedPerson, index) =>
                SlackBlockBuilder.CreateSectionWithText($"{SlackNumberEmojis.From(index + 1)} <@{queuedPerson.UserId}>")).ToList();
            var response = new SlackSlashResponseDto
            {
                ResponseType = SlackMessageType.InChannel,
                Blocks = queue
            };
            return response;
        }
    }
}