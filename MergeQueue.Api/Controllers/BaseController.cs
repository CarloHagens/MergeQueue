﻿using MergeQueue.Api.Builders;
using MergeQueue.Api.Dtos;
using MergeQueue.Api.Entities;
using MergeQueue.Api.Filters;
using MergeQueue.Api.Types;
using Microsoft.AspNetCore.Mvc;

namespace MergeQueue.Api.Controllers
{
    [ApiController]
    [TypeFilter(typeof(AuthenticationFilter))]
    public class BaseController : ControllerBase
    {
        protected static IEnumerable<SlackBlockDto> CreateShowQueueResponseBlocks(IEnumerable<User> queuedUsers)
        {
            return queuedUsers.Select((queuedUser, index) =>
                SlackBlockBuilder
                    .CreateSection()
                    .WithText(ResponseMessages.UserQueuePosition(index, queuedUser.UserId))
            ).ToList();
        }

        protected static string CreateJoinQueueResponseText(User user, IReadOnlyCollection<User> queuedUsers) =>
            queuedUsers.Count == 1
                ? ResponseMessages.UserFirstInQueue(user.UserId)
                : ResponseMessages.UserJoinedQueue(user.UserId);

        protected static string CreateLeaveQueueResponseText(User user, IReadOnlyCollection<User> queuedUsers)
        {
            var responseText = ResponseMessages.UserLeftQueue(user.UserId);
            return CreateRemoveQueueResponseText(user, queuedUsers, responseText);
        }

        protected static string CreateKickQueueResponseText(User user, IReadOnlyCollection<User> queuedUsers)
        {
            var responseText = ResponseMessages.UserKickedFromTheQueue(user.UserId);
            return CreateRemoveQueueResponseText(user, queuedUsers, responseText);
        }

        protected static string CreateJumpQueueResponseText(User user, IReadOnlyCollection<User> queuedUsers) =>
            queuedUsers.Count == 1
                ? ResponseMessages.UserFirstInQueue(user.UserId)
                : ResponseMessages.UserJumpedQueue(user.UserId);

        // This method expects the queue before the to be removed user is removed.
        protected static string CreateRemoveQueueResponseText(User user, IReadOnlyCollection<User> queuedUsers, string responseText)
        {
            // In case queue was already empty then the queue order does not change. If user to be removed is not first in queue then the queue order does not change.
            if (queuedUsers.Count == 0 || queuedUsers.First().UserId != user.UserId)
            {
                return responseText;
            }
            // If there is more than 1 user and the user was first in the queue then the order of the queue changes.
            if (queuedUsers.Count > 1)
            {
                responseText += ResponseMessages.UserTurnArrived(queuedUsers.Skip(1).First().UserId);
            }
            // If there was only 1 user in the queue then it is now empty.
            else
            {
                responseText += ResponseMessages.QueueNowEmpty;
            }

            return responseText;
        }
    }
}
