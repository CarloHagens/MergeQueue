using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MergeQueue.Dtos;
using MergeQueue.Types;

namespace MergeQueue.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Queue2Controller : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public Queue2Controller(HttpClient httpClient)
        {
            _httpClient = httpClient;
            if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization",
                    "Bearer xoxb-2174785981490-2171724521813-wsp1iua7epk4dI3IQDUhEaft");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] SlackInteractivityRequestDto request)
        {
            var requestObject = JsonSerializer.Deserialize<SlackInteractivityRequestPayloadDto>(request.payload);
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
            var responseBody = new SlackInteractivityViewOpenDto
            {
                TriggerId = triggerId,
                View = SelectChannelAndUserView(false)
            };
            var content = new StringContent(JsonSerializer.Serialize(responseBody), Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("https://slack.com/api/views.open", content);
        }

        private async Task UpdateView(string triggerId)
        {
            var responseBody = new SlackInteractivityViewOpenDto
            {
                TriggerId = triggerId,
                View = SelectChannelAndUserView(true)
            };
            var content = new StringContent(JsonSerializer.Serialize(responseBody), Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("https://slack.com/api/views.update", content);
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
            var responseBody = new SlackInteractivityUpdateStepDto
            {
                WorkFlowStepEditId = requestObject.WorkflowStep.WorkflowStepEditId
            };

            var selectedUser = requestObject.View.State.Values.SelectMany(block => block.Value)
                .FirstOrDefault(action => action.Key == "select_user").Value.SelectedUser;
            var selectedChannel = requestObject.View.State.Values.SelectMany(block => block.Value)
                .FirstOrDefault(action => action.Key == "select_channel").Value.SelectedChannel;

            responseBody.Inputs = new Dictionary<string, SlackInputValueDto>
            {
                {"selected_user", new SlackInputValueDto{ Value = selectedUser}},
                {"selected_channel", new SlackInputValueDto{ Value = selectedChannel}}
            };

            var content = new StringContent(JsonSerializer.Serialize(responseBody), Encoding.UTF8,"application/json");
            var response = await _httpClient.PostAsync("https://slack.com/api/workflows.updateStep", content);
            var responseContent = await response.Content.ReadAsStringAsync();
        }
    }
}
