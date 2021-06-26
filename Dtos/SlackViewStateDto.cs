using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackViewStateDto
    {
        [JsonPropertyName("values")]
        public Dictionary<string, Dictionary<string, SlackViewStateValue>> Values { get; set; }
    }
}