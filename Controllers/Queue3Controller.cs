using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using MergeQueue.Dtos;
using MergeQueue.Entities;
using MergeQueue.Repositories;
using MergeQueue.Types;

namespace MergeQueue.Controllers
{
    [Route("[controller]")]
    public class Queue3Controller : BaseController
    {
        public Queue3Controller(IQueueRepository repository, HttpClient httpClient) 
            : base(repository, httpClient)
        {
        }

        [HttpPost]
        public ActionResult Post([FromBody] SlackEventSubscriptionRequestDto request)
        {
            if (request.Type == SlackEventTypes.UrlVerification)
            {
                return Ok(request.Challenge);
            }
            if (request.Type == SlackEventTypes.EventCallback 
                && request.Event.Type == "workflow_step_execute"
                && request.Event.CallbackId == Commands.Join)
            {
                var user = new User
                {
                    ChannelId = request.Event.WorkflowStep.Inputs["selected_channel"].Value,
                    UserId = request.Event.WorkflowStep.Inputs["selected_user"].Value
                };
                Repository.AddUser(user);
                return Ok();
            }

            return Ok();
        }
    }
}
