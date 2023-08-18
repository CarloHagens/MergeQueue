namespace MergeQueue.Api.Types
{
    public static class SlackApiEndpoints
    {
        public static string OpenView => "views.open";
        // TODO: Part of UI validation.
        //public static string UpdateView => "views.update";
        public static string UpdateWorkflowStep => "workflows.updateStep";
        public static string SendMessage => "chat.postMessage";
        public static string SendEphemeralMessage => "chat.postEphemeral";
    }
}
