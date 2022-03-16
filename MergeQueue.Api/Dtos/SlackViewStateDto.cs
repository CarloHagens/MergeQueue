using System.Collections.Generic;

namespace MergeQueue.Api.Dtos
{
    public class SlackViewStateDto
    {
        public Dictionary<string, Dictionary<string, SlackViewStateValue>> Values { get; set; }
    }
}