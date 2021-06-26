﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MergeQueue.Dtos
{
    public class SlackSlashResponseDto
    {
        [JsonPropertyName("response_type")]
        public string ResponseType { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("blocks")]
        public IEnumerable<SlackBlockDto> Blocks { get; set; }
    }
}
