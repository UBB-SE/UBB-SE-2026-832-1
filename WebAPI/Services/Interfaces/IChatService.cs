using ClassLibrary.DTOs;

namespace WebApi.Services.Interfaces
{
    public interface IChatService
    {
        Task AddMessageAsync(int conversationId, Guid senderId, string text, bool isNutritionist);
        Task<IEnumerable<ConversationDto>> GetAllConversationsAsync();
        Task<IEnumerable<ConversationDto>> GetConversationsWhereNutritionistRespondedAsync(Guid nutritionistId);
        Task<IEnumerable<ConversationDto>> GetConversationsWithMessagesAsync();
        Task<IEnumerable<ConversationDto>> GetConversationsWithUserMessagesAsync();
        Task<IEnumerable<MessageDto>> GetMessagesForConversationAsync(int conversationId);
        Task<ConversationDto> GetOrCreateConversationForUserAsync(Guid userId);
    }
}