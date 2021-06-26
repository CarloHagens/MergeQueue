using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using MergeQueue.Dtos;
using MergeQueue.Entities;
using MergeQueue.Repositories;
using MergeQueue.Types;

namespace MergeQueue.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Queue3Controller : ControllerBase
    {
        private readonly IQueueRepository _repository;
        private readonly HttpClient _httpClient;

        public Queue3Controller(IQueueRepository repository, HttpClient httpClient)
        {
            _repository = repository;
            _httpClient = httpClient;
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
                _repository.AddUser(user);
                return Ok();
            }

            return Ok();
        }
    }
}
