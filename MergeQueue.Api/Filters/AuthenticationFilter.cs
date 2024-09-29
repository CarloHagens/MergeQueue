using MergeQueue.Api.Extensions;
using MergeQueue.Api.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;
using System.Text;

namespace MergeQueue.Api.Filters
{
    public class AuthenticationFilter : IAsyncActionFilter
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public AuthenticationFilter(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("Starting authentication workflow");
            var request = context.HttpContext.Request;
            request.Headers.TryGetValue("X-Slack-Request-Timestamp", out var slackRequestTimestamp);
            request.Headers.TryGetValue("X-Slack-Signature", out var slackSignature);

            _logger.LogInformation($"Timestamp: {slackRequestTimestamp}");
            _logger.LogInformation($"Slack Signature: {slackSignature}");

            if (slackRequestTimestamp.DoesNotExist() || string.IsNullOrEmpty(slackRequestTimestamp.FirstOrDefault())
                || slackSignature.DoesNotExist() || string.IsNullOrEmpty(slackSignature.FirstOrDefault()))
            {
                _logger.LogInformation("Failed to authenticate.");
                context.Result = new UnauthorizedResult();
                return;
            }

            request.Body.Position = 0;
            string body;
            using (var reader = new StreamReader(request.Body, leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            _logger.LogInformation($"Body: {body}");
            var initialSignature = $"v0:{slackRequestTimestamp.First()}:{body}";

            string calculatedSignature;
            var slackApiSettings = _configuration.GetSection(nameof(SlackApiSettings)).Get<SlackApiSettings>();
            using (var hash = new HMACSHA256(Encoding.UTF8.GetBytes(slackApiSettings.SigningSecret)))
            {
                var computedHash = hash.ComputeHash(Encoding.UTF8.GetBytes(initialSignature));
                calculatedSignature = $"v0={computedHash.ToHexString()}";
            }

            _logger.LogInformation($"Calculated Signature: {calculatedSignature}");
            if (calculatedSignature != slackSignature.First())
            {
                _logger.LogInformation("Failed to authenticate.");
                context.Result = new UnauthorizedResult();
                return;
            }
            _logger.LogInformation("Successfully authenticated request.");
            await next();
        }
    }
}
