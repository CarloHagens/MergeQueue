namespace MergeQueue.Api.Types
{
    public static class SlackApiEndpoints
    {
        public static string OpenView => "https://slack.com/api/views.open";
        // TODO: Part of UI validation.
        //public static string UpdateView => "https://slack.com/api/views.update";
        public static string UpdateWorkflowStep => "https://slack.com/api/workflows.updateStep";
        public static string SendMessage => "https://slack.com/api/chat.postMessage";
        public static string SendEphemeralMessage => "https://slack.com/api/chat.postEphemeral";
    }
}
