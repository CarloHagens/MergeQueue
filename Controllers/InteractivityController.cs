using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MergeQueue.Dtos;
using MergeQueue.Repositories;
using MergeQueue.Types;
using Microsoft.Extensions.Configuration;

namespace MergeQueue.Controllers
{
    [Route("[controller]")]
    public class InteractivityController : BaseController
    {
        public InteractivityController(IConfiguration  configuration, IQueueRepository queueRepository, HttpClient httpClient) 
            : base(configuration, queueRepository, httpClient)
        {
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] SlackInteractivityRequestDto request)
        {
            var serializationSettings = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = new SnakeCaseNamingPolicy()
            };
            var requestObject = JsonSerializer.Deserialize<SlackInteractivityRequestPayloadDto>(request.payload, serializationSettings);
            if (requestObject?.Type == SlackInteractivityTypes.WorkflowStepEdit)
            {
                await OpenView(requestObject?.TriggerId);
                return Ok();
            }
            if (requestObject?.Type == SlackInteractivityTypes.ViewSubmission)
            {
                await UpdateStep(requestObject);
                return Ok();
            }

            if (requestObject?.Type == SlackInteractivityTypes.BlockActions)
            {
                return Ok();
            }

            return Ok();
        }

        private async Task OpenView(string triggerId)
        {
            var body = new SlackInteractivityViewOpenDto
            {
                TriggerId = triggerId,
                View = SelectChannelAndUserView(false)
            };
            await PostToUrlWithBody(SlackApiEndpoints.OpenView, body);
        }

        private async Task UpdateView(string triggerId)
        {
            var body = new SlackInteractivityViewOpenDto
            {
                TriggerId = triggerId,
                View = SelectChannelAndUserView(true)
            };
            await PostToUrlWithBody(SlackApiEndpoints.UpdateView, body);
        }

        private static SlackViewDto SelectChannelAndUserView(bool submitDisabled)
        {
            return new ()
            {
                Type = SlackInteractivityViewType.WorkflowStep,
                SubmitDisabled = submitDisabled,
                Blocks = new List<SlackBlockDto>
                {
                    new()
                    {
                        Type = SlackBlockType.Section,
                        Text = new SlackBlockTextDto
                        {
                            Type = SlackTextType.Markdown,
                            Text = "Pick a user from the list."
                        },
                        Accessory = new SlackBlockAccessoryDto
                        {
                            ActionId = "select_user",
                            Type = SlackBlockType.UsersSelect,
                            Placeholder = new SlackBlockTextDto
                            {
                                Type = SlackTextType.PlainText,
                                Text = "Select user"
                            }
                        }
                    },
                    new()
                    {
                        Type = SlackBlockType.Section,
                        Text = new SlackBlockTextDto
                        {
                            Type = SlackTextType.Markdown,
                            Text = "Pick a channel from the list."
                        },
                        Accessory = new SlackBlockAccessoryDto
                        {
                            ActionId = "select_channel",
                            Type = SlackBlockType.ChannelsSelect,
                            Placeholder = new SlackBlockTextDto
                            {
                                Type = SlackTextType.PlainText,
                                Text = "Select channel"
                            }
                        }
                    }
                }
            };
        }

        private async Task UpdateStep(SlackInteractivityRequestPayloadDto requestObject)
        {
            var body = new SlackInteractivityUpdateStepDto
            {
                WorkflowStepEditId = requestObject.WorkflowStep.WorkflowStepEditId
            };

            var selectedUser = requestObject.View.State.Values.SelectMany(block => block.Value)
                .FirstOrDefault(action => action.Key == "select_user").Value.SelectedUser;
            var selectedChannel = requestObject.View.State.Values.SelectMany(block => block.Value)
                .FirstOrDefault(action => action.Key == "select_channel").Value.SelectedChannel;

            body.Inputs = new Dictionary<string, SlackInputValueDto>
            {
                {"selected_user", new SlackInputValueDto{ Value = selectedUser}},
                {"selected_channel", new SlackInputValueDto{ Value = selectedChannel}}
            };

            await PostToUrlWithBody(SlackApiEndpoints.UpdateWorkflowStep, body);
        }
    }
}
