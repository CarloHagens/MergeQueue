using MergeQueue.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using MergeQueue.Dtos;
using MergeQueue.Entities;

namespace MergeQueue.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly IQueueRepository _repository;
        private readonly HttpClient _httpClient;
        private readonly string _queueCommands = "" +
                                             $"`/queue {Commands.Show}` - Show a queue for this channel\n" +
                                             $"`/queue {Commands.Join}` - Join the queue\n" +
                                             $"`/queue {Commands.Leave}` - Leave the queue\n" +
                                             $"`/queue {Commands.Help}` - Show all available commands";

        public QueueController(IQueueRepository repository, HttpClient httpClient)
        {
            _repository = repository;
            _httpClient = httpClient;
        }

        [HttpPost]
        public ActionResult Post([FromForm] SlackSlashRequestDto request)
        {
            if (request.text != null && request.text.ToLowerInvariant() == "show")
            {
                var queuedPeople = _repository.GetUsersForChannel(request.channel_id);
                if (queuedPeople.Count == 0)
                {
                    return Ok(CreateEphemeralResponseWithText("The queue is currently empty."));
                }
                var queue = queuedPeople.Select((queuedPerson, index) =>
                    CreateSectionWithText($"{SlackNumberEmojis.From(index + 1)} <@{queuedPerson.UserId}>")).ToList();
                var response = new SlackSlashResponseDto
                {
                    ResponseType = SlackSlashResponseType.InChannel,
                    Blocks = queue
                };
                InvokeResponseUrl(request.response_url, response);
                return Ok();
            }

            if (request.text != null && request.text.ToLowerInvariant() == "join")
            {
                var wasUserAdded = _repository.AddUser(request.ToUser());
                if (!wasUserAdded)
                {
                    return Ok(CreateEphemeralResponseWithText("You are already in the queue."));
                }
                var queuedUsers = _repository.GetUsersForChannel(request.channel_id);
                var responseText = queuedUsers.Count == 1
                    ? $"<@{request.user_id}> is now first in the queue!"
                    : $"<@{request.user_id}> joined the queue.";
                var response = CreateChannelResponseWithText(responseText);
                InvokeResponseUrl(request.response_url, response);
                return Ok();
            }

            if (request.text != null && request.text.ToLowerInvariant() == "leave")
            {
                var responseText = $"<@{request.user_id}> left the queue.";
                var queuedPeople = _repository.GetUsersForChannel(request.channel_id);
                if (queuedPeople.Count > 0 && queuedPeople.First().UserId == request.user_id)
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
                var wasPersonRemoved = _repository.RemoveUser(request.ToUser());
                if (!wasPersonRemoved)
                {
                    return Ok(CreateEphemeralResponseWithText("You are not in the queue."));
                }

                var response = CreateChannelResponseWithText(responseText);
                InvokeResponseUrl(request.response_url, response);
                return Ok();
            }

            var response2 = new SlackSlashResponseDto
            {
                ResponseType = SlackSlashResponseType.Ephemeral,
                Blocks = new List<SlackSlashResponseBlock>
                    {
                        CreateSectionWithText(":wave: Need some help with `/queue`?"),
                        CreateSectionWithText("*Available commands*"),
                        CreateSectionWithText(_queueCommands)
                    }
            };
            return Ok(response2);
        }

        private void InvokeResponseUrl(string responseUrl, object response)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, responseUrl)
            {
                Content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json")
            };
            _httpClient.Send(requestMessage);
        }

        private SlackSlashResponseBlock CreateSectionWithText(string text)
        {
            return new()
            {
                Type = SlackSlashResponseBlockType.Section,
                Text = new SlackSlashResponseText
                {
                    Type = SlackSlashResponseTextType.Markdown,
                    Text = text
                }
            };
        }

        private SlackSlashResponseDto CreateEphemeralResponseWithText(string text)
        {
            return new()
            {
                ResponseType = SlackSlashResponseType.Ephemeral,
                Text = text
            };
        }

        private SlackSlashResponseDto CreateChannelResponseWithText(string text)
        {
            return new()
            {
                ResponseType = SlackSlashResponseType.InChannel,
                Text = text
            };
        }
    }
}
