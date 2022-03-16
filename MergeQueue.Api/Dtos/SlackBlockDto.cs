namespace MergeQueue.Api.Dtos
{
    public class SlackBlockDto
    {
        public string Type { get; set; }
        public SlackBlockTextDto Text { get; set; }
        public SlackBlockAccessoryDto Accessory { get; set; }
    }
}