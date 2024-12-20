namespace MergeQueue.Api.Services
{
    public static class SlackServiceEndpoints
    {
        public static string FunctionsCompleteSuccess => "functions.completeSuccess";
        public static string SendMessage => "chat.postMessage";
        public static string SendEphemeralMessage => "chat.postEphemeral";
    }
}
