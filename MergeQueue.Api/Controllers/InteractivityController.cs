using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MergeQueue.Api.Dtos;
using MergeQueue.Api.Types;
using MergeQueue.Api.Builders;
using System.Text.Json.Serialization;
using MergeQueue.Api.Services;

namespace MergeQueue.Api.Controllers
{
    [Route("[controller]")]
    public class InteractivityController : BaseController
    {
        private readonly ISlackService slackService;

        public InteractivityController(ISlackService slackService)
        {
            this.slackService = slackService;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] SlackInteractivityRequestDto request)
        {
            var requestObject = DeserializePayload(request);

            if (requestObject?.Type == SlackInteractivityTypes.WorkflowStepEdit)
            {
                await OpenView(requestObject.TriggerId);
            }
            else if (requestObject?.Type == SlackInteractivityTypes.ViewSubmission)
            {
                await UpdateStep(requestObject);
            }

            // TODO: Part of UI validation.
            //if (requestObject?.Type == SlackInteractivityTypes.BlockActions)
            //{
            //    return Ok();
            //}

            return Ok();
        }

        private static SlackInteractivityRequestPayloadDto? DeserializePayload(SlackInteractivityRequestDto request)
        {
            var serializationSettings = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = new SnakeCaseNamingPolicy()
            };
            return JsonSerializer.Deserialize<SlackInteractivityRequestPayloadDto>(request.payload, serializationSettings);
        }

        private async Task OpenView(string triggerId)
        {
            var body = new SlackInteractivityViewOpenDto
            {
                TriggerId = triggerId,
                View = SelectChannelAndUserView(false)
            };
            await slackService.OpenView(body);
        }

        // TODO: Part of UI validation.
        //private async Task UpdateView(string triggerId)
        //{
        //    var body = new SlackInteractivityViewOpenDto
        //    {
        //        TriggerId = triggerId,
        //        View = SelectChannelAndUserView(true)
        //    };
        //    await PostToUrlWithBody(SlackApiEndpoints.UpdateView, body);
        //}

        private static SlackViewDto SelectChannelAndUserView(bool submitDisabled)
        {
            return new()
            {
                Type = SlackInteractivityViewType.WorkflowStep,
                SubmitDisabled = submitDisabled,
                Blocks = new List<SlackBlockDto>
                {
                    SlackBlockBuilder
                        .CreateSection()
                        .WithText(ResponseMessages.SelectChannel)
                        .WithAccessory(
                            ActionIdTypes.SelectChannel,
                            SlackBlockType.ChannelsSelect,
                            ResponseMessages.SelectChannelPlaceholder
                    ),
                    SlackBlockBuilder
                        .CreateSection()
                        .WithText(ResponseMessages.SelectUser)
                        .WithAccessory(
                            ActionIdTypes.SelectUser,
                            SlackBlockType.UsersSelect,
                            ResponseMessages.SelectUserPlaceholder
                    )
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
                .FirstOrDefault(action => action.Key == ActionIdTypes.SelectUser).Value.SelectedUser;
            var selectedChannel = requestObject.View.State.Values.SelectMany(block => block.Value)
                .FirstOrDefault(action => action.Key == ActionIdTypes.SelectChannel).Value.SelectedChannel;

            body.Inputs = new Dictionary<string, SlackInputValueDto>
            {
                {InputTypes.SelectedChannel, new SlackInputValueDto{ Value = selectedChannel}},
                {InputTypes.SelectedUser, new SlackInputValueDto{ Value = selectedUser}}
            };

            await slackService.UpdateWorkflowStep(body);
        }
    }
}
