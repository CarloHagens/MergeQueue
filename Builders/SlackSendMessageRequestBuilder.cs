using System.Collections.Generic;
using MergeQueue.Dtos;

namespace MergeQueue.Builders
{
    public static class SlackSendMessageRequestBuilder
    {
        public static SlackSendMessageRequestDto CreateSendMessageRequestToChannel(string channelId)
        {
            return new()
            {
                Channel = channelId
            };
        }

        public static SlackSendMessageRequestDto ToUser(this SlackSendMessageRequestDto request, string userId)
        {
            request.User = userId;
            return request;
        }

        public static SlackSendMessageRequestDto WithText(this SlackSendMessageRequestDto request, string text)
        {
            request.Text = text;
            return request;
        }

        public static SlackSendMessageRequestDto WithBlocks(this SlackSendMessageRequestDto request, IEnumerable<SlackBlockDto> blocks)
        {
            request.Blocks = blocks;
            return request;
        }
    }
}
