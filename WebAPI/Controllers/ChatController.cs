using Microsoft.AspNetCore.Mvc;
using ClassLibrary.DTOs;
using WebApi.IServices;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService chatService;

        public ChatController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllConversations()
        {
            var conversations = await this.chatService.GetAllConversationsAsync();
            return this.Ok(conversations);
        }

        [HttpPost("user/{userId:guid}")]
        public async Task<IActionResult> GetOrCreateConversationForUser(Guid userId)
        {
            var conversation = await this.chatService.GetOrCreateConversationForUserAsync(userId);

            if (conversation is null)
            {
                return this.NotFound();
            }

            return this.Ok(conversation);
        }

        [HttpGet("{conversationId:int}/messages")]
        public async Task<IActionResult> GetMessages(int conversationId)
        {
            var messages = await this.chatService.GetMessagesForConversationAsync(conversationId);
            return this.Ok(messages);
        }

        [HttpGet("with-messages")]
        public async Task<IActionResult> GetConversationsWithMessages()
        {
            var conversations = await this.chatService.GetConversationsWithMessagesAsync();
            return this.Ok(conversations);
        }

        [HttpGet("with-user-messages")]
        public async Task<IActionResult> GetConversationsWithUserMessages()
        {
            var conversations = await this.chatService.GetConversationsWithUserMessagesAsync();
            return this.Ok(conversations);
        }

        [HttpGet("nutritionist/{nutritionistId:guid}/responded")]
        public async Task<IActionResult> GetConversationsWhereNutritionistResponded(Guid nutritionistId)
        {
            var conversations = await this.chatService.GetConversationsWhereNutritionistRespondedAsync(nutritionistId);
            return this.Ok(conversations);
        }

        [HttpPost("{conversationId:int}/messages")]
        public async Task<IActionResult> AddMessage(int conversationId, [FromBody] AddMessageRequestDto request)
        {
            await this.chatService.AddMessageAsync(conversationId, request.SenderId, request.Text, request.IsNutritionist);

            return this.NoContent();
        }
    }
}