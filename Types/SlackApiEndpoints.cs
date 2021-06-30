namespace MergeQueue.Types
{
    public static class SlackApiEndpoints
    {
        public static string OpenView => "https://slack.com/api/views.open";
        public static string UpdateView => "https://slack.com/api/views.update";
        public static string UpdateWorkflowStep => "https://slack.com/api/workflows.updateStep";
        public static string SendMessage => "https://slack.com/api/chat.postMessage";
    }
}
