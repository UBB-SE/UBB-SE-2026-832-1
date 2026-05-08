using ClassLibrary.DTOs;

namespace WebApi.IServices
{
    public interface IChatService
    {
        Task AddMessageAsync(int conversationId, int senderId, string text, bool isNutritionist);
        Task<IEnumerable<ConversationDto>> GetAllConversationsAsync();
        Task<IEnumerable<ConversationDto>> GetConversationsWhereNutritionistRespondedAsync(int nutritionistId);
        Task<IEnumerable<ConversationDto>> GetConversationsWithMessagesAsync();
        Task<IEnumerable<ConversationDto>> GetConversationsWithUserMessagesAsync();
        Task<IEnumerable<MessageDto>> GetMessagesForConversationAsync(int conversationId);
        Task<ConversationDto?> GetOrCreateConversationForUserAsync(int userId);
    }
}