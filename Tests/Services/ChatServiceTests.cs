using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using FluentAssertions;
using NSubstitute;
using WebApi.Services;
using Xunit;

namespace WebApi.Tests.Services
{
    public class ChatServiceTests
    {
        private readonly IChatRepository _mockRepo;
        private readonly ChatService _service;

        public ChatServiceTests()
        {
            _mockRepo = Substitute.For<IChatRepository>();
            _service = new ChatService(_mockRepo);
        }

        [Fact]
        public async Task GetOrCreateConversationForUserAsync_ReturnsConversationDtoWithCorrectUser()
        {
           
            var userId = 5;
            var expectedConversation = new Conversation
            {
                Id = 1,
                HasUnanswered = false,
                User = new User { UserId = userId, Username = "Marcel" }
            };
            _mockRepo.GetOrCreateConversationForUserAsync(userId).Returns(expectedConversation);

            
            var result = await _service.GetOrCreateConversationForUserAsync(userId);

            
            result.Should().NotBeNull();
            result!.UserId.Should().Be(userId);
            result.UserName.Should().Be("Marcel");
            result.ConversationId.Should().Be(1);
        }

        [Fact]
        public async Task GetAllConversationsAsync_WhenEmpty_ReturnsEmptyList()
        {
            _mockRepo.GetAllConversationsAsync().Returns(new List<Conversation>());

            var result = await _service.GetAllConversationsAsync();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task AddMessageAsync_CallsRepositoryCorrectly()
        {
            await _service.AddMessageAsync(1, 10, "Hello Nutritionist", isNutritionist: false);

            await _mockRepo.Received(1).AddMessageAsync(1, 10, "Hello Nutritionist", false);
        }
    }
}