using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MergeQueue.Api.Extensions;
using MergeQueue.Api.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace MergeQueue.Api.Filters
{
    public class AuthenticationFilter : IAsyncActionFilter
    {
        private readonly IConfiguration _configuration;
        public AuthenticationFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;
            request.Headers.TryGetValue("X-Slack-Request-Timestamp", out var slackRequestTimeStamp);
            request.Headers.TryGetValue("X-Slack-Signature", out var slackSignature);

            if (slackRequestTimeStamp.DoesNotExist() || string.IsNullOrEmpty(slackRequestTimeStamp.FirstOrDefault())
                || slackSignature.DoesNotExist() || string.IsNullOrEmpty(slackSignature.FirstOrDefault()))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            request.Body.Position = 0;
            string body;
            using (var reader = new StreamReader(request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            var signature = $"v0:{slackRequestTimeStamp.First()}:{body}";

            string signingSecret;
            var slackApiSettings = _configuration.GetSection(nameof(SlackApiSettings)).Get<SlackApiSettings>();
            using (var hash = new HMACSHA256(Encoding.UTF8.GetBytes(slackApiSettings.SigningSecret)))
            {
                var computedHash = hash.ComputeHash(Encoding.UTF8.GetBytes(signature));
                signingSecret = $"v0={computedHash.ToHexString()}";
            }

            if (signingSecret != slackSignature.First())
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            await next();
        }
    }
}
