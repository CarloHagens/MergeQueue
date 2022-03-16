using System.Collections.Generic;
using MergeQueue.Api.Dtos;
using MergeQueue.Api.Types;

namespace MergeQueue.Api.Builders
{
    public static class SlackSlashResponseBuilder
    {
        public static SlackSlashResponseDto CreateEphemeralResponse()
        {
            return new()
            {
                ResponseType = SlackMessageType.Ephemeral
            };
        }

        public static SlackSlashResponseDto CreateChannelResponse()
        {
            return new()
            {
                ResponseType = SlackMessageType.InChannel
            };
        }

        public static SlackSlashResponseDto WithText(this SlackSlashResponseDto response, string text)
        {
            response.Text = text;
            return response;
        }

        public static SlackSlashResponseDto WithBlocks(this SlackSlashResponseDto response, IEnumerable<SlackBlockDto> blocks)
        {
            response.Blocks = blocks;
            return response;
        }
    }
}
