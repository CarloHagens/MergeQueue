using MergeQueue.Api.Repositories;
using MergeQueue.Api.Entities;
using MergeQueue.Api.Controllers;
using MergeQueue.Api.Dtos;
using MergeQueue.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MergeQueue.Tests
{
    public class KickCommandTests
    {
        private SlashCommandsController slashCommandsController;
        private Mock<ISlackService> mockSlackService;

        public KickCommandTests()
        {
            var mockQueueRepo = new Mock<IQueueLookup>();
            mockSlackService = new Mock<ISlackService>();
            var mockLogger = new Mock<ILogger>();
            mockQueueRepo.Setup(x => x.GetUsersForChannel(It.IsAny<string>())).ReturnsAsync(
                    new List<User> 
                    {
                        new User
                        {
                            ChannelId = "test-channel",
                            UserId = "test-user"
                        }
                    }
            );
            slashCommandsController = new SlashCommandsController(mockQueueRepo.Object, mockSlackService.Object, mockLogger.Object);
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