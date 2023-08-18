namespace MergeQueue.Api.Services
{
    public static class SlackServiceEndpoints
    {
        private static readonly string baseUrl = "https://slack.com/api/";
        public static string OpenView => $"{baseUrl}views.open";
        // TODO: Part of UI validation.
        //public static string UpdateView => "views.update";
        public static string UpdateWorkflowStep => $"{baseUrl}workflows.updateStep";
        public static string SendMessage => $"{baseUrl}chat.postMessage";
        public static string SendEphemeralMessage => $"{baseUrl}chat.postEphemeral";
    }
}
