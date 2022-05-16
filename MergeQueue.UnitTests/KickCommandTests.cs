using MergeQueue.Api.Repositories;
using MergeQueue.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using MergeQueue.Api.Controllers;
using MergeQueue.Api.Dtos;

namespace MergeQueue.Tests
{
    public class KickCommandTests
    {
        private SlashCommandsController slashCommandsController;

        public KickCommandTests()
        {
            var mockRepo = new Mock<IQueueRepository>();
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
            var httpClient = new HttpClient();
            slashCommandsController = new SlashCommandsController(config, mockRepo.Object, httpClient);
        }

        [Fact]
        public async Task UserWithPipe()
        {
            // Arrange
            var user = "kick <@abc123|test>";
            var request = new SlackSlashRequestDto
            {
                text = user
            };

            // Act
            OkObjectResult response = (OkObjectResult) await slashCommandsController.Post(request);
            SlackSlashResponseDto responseBody = response.Value as SlackSlashResponseDto;

            // Assert
            Assert.Equal("<@abc123> is not in the queue.", responseBody.Text);
        }

        [Fact]
        public async Task UserWithoutPipe()
        {
            // Arrange
            var user = "kick <@abc123>";
            var request = new SlackSlashRequestDto
            {
                text = user
            };

            // Act
            OkObjectResult response = (OkObjectResult)await slashCommandsController.Post(request);
            SlackSlashResponseDto responseBody = response.Value as SlackSlashResponseDto;

            // Assert
            Assert.Equal("<@abc123> is not in the queue.", responseBody.Text);
        }

        [Fact]
        public async Task UserWithPipeAndStrangeCharacters()
        {
            // Arrange
            var user = "kick <@UM9R4UY30|marcin.sak>";
            var request = new SlackSlashRequestDto
            {
                text = user
            };

            // Act
            OkObjectResult response = (OkObjectResult)await slashCommandsController.Post(request);
            SlackSlashResponseDto responseBody = response.Value as SlackSlashResponseDto;

            // Assert
            Assert.Equal("<@UM9R4UY30> is not in the queue.", responseBody.Text);
        }
    }
}