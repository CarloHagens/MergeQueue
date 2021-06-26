using System.Collections.Generic;

namespace MergeQueue.Dtos
{
    public class SlackViewStateDto
    {
        public Dictionary<string, Dictionary<string, SlackViewStateValue>> Values { get; set; }
    }
}