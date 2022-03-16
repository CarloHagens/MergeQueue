namespace MergeQueue.Api.Dtos
{
    public class SlackBlockAccessoryDto
    {
        public string ActionId { get; set; }
        public string Type { get; set; }
        public SlackBlockTextDto Placeholder { get; set; }
    }
}