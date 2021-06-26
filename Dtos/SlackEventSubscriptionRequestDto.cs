namespace MergeQueue.Dtos
{
    public class SlackEventSubscriptionRequestDto
    {
        public string Token { get; set; }
        public string Challenge { get; set; }
        public string Type { get; set; }
        public SlackEventDto Event { get; set; }
    }
}
