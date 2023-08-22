using MergeQueue.Api.Repositories;
using MergeQueue.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using MergeQueue.Api.Controllers;
using MergeQueue.Api.Dtos;
using System.Linq;
using MergeQueue.Api.Services;

namespace MergeQueue.Tests
{
    public class JoinCommandTests
    {
        private SlashCommandsController slashCommandsController;
        private Mock<IQueueLookup> mockRepo;
        private Mock<ISlackService> mockSlackService;

        public JoinCommandTests()
        {
            mockRepo = new Mock<IQueueLookup>();
            mockSlackService = new Mock<ISlackService>();
            mockRepo.Setup(x => x.GetUsersForChannel(It.IsAny<string>())).ReturnsAsync(
                    new List<User>
                    {
                        new User
                        {
                            ChannelId = "test-channel",
                            UserId = "test-user"
                        }
                    }
            );
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfiguration config = new ConfigurationBuilder()
               .SetBasePath(projectPath)
               .AddJsonFile("appsettings.json")
               .Build();
            slashCommandsController = new SlashCommandsController(mockRepo.Object, mockSlackService.Object);
        }

        [Fact]
        public async Task JoinSucceeds()
        {
            // Arrange
            var user = "join";
            mockRepo.Setup(x => x.AddUser(It.IsAny<User>())).ReturnsAsync(true);
            var request = new SlackSlashRequestDto
            {
                text = user
            };

            // Act
            OkObjectResult response = (OkObjectResult)await slashCommandsController.Post(request);
            SlackSlashResponseDto responseBody = response.Value as SlackSlashResponseDto;

            // Assert
            Assert.Equal(":first_place_medal: <@test-user>", responseBody.Blocks.ToList()[0].Text.Text);

            mockSlackService.Verify(mock => mock.SendResponse(null, It.Is<SlackSlashResponseDto>(p =>
                p.Text == "<@> is now first in the queue!"
                && p.ResponseType == "in_channel"
            )), Times.Once());
        }

        [Fact]
        public async Task JoinFails()
        {
            // Arrange
            var user = "join";
            mockRepo.Setup(x => x.AddUser(It.IsAny<User>())).ReturnsAsync(false);
            var request = new SlackSlashRequestDto
            {
                text = user
            };

            // Act
            OkObjectResult response = (OkObjectResult)await slashCommandsController.Post(request);
            SlackSlashResponseDto responseBody = response.Value as SlackSlashResponseDto;

            // Assert
            Assert.Equal("You are already in the queue.", responseBody.Text);
        }

        [Fact]
        public async Task JoinAtPositionSucceeds()
        {
            // Arrange
            var user = "join 1";
            mockRepo.Setup(x => x.AddUser(It.IsAny<User>(), It.IsAny<int>())).ReturnsAsync(true);
            var request = new SlackSlashRequestDto
            {
                text = user
            };

            // Act
            OkObjectResult response = (OkObjectResult)await slashCommandsController.Post(request);
            SlackSlashResponseDto responseBody = response.Value as SlackSlashResponseDto;

            // Assert
            Assert.Equal(":first_place_medal: <@test-user>", responseBody.Blocks.ToList()[0].Text.Text);
        }

        [Fact]
        public async Task JoinAtPositionFails()
        {
            // Arrange
            var user = "join 1";
            mockRepo.Setup(x => x.AddUser(It.IsAny<User>(), It.IsAny<int>())).ReturnsAsync(false);
            var request = new SlackSlashRequestDto
            {
                text = user
            };

            // Act
            OkObjectResult response = (OkObjectResult)await slashCommandsController.Post(request);
            SlackSlashResponseDto responseBody = response.Value as SlackSlashResponseDto;

            // Assert
            Assert.Equal("You are already in the queue.", responseBody.Text);
        }
    }
}