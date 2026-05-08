using ClassLibrary.DTOs;

namespace WinUI.Services
{
    public interface IChatService
    {
        Task AddMessageAsync(int conversationId, AddMessageRequestDto request);
        Task<IReadOnlyList<ConversationDto>> GetAllConversationsAsync();
        Task<IReadOnlyList<ConversationDto>> GetConversationsWhereNutritionistRespondedAsync(int nutritionistId);
        Task<IReadOnlyList<ConversationDto>> GetConversationsWithMessagesAsync();
        Task<IReadOnlyList<ConversationDto>> GetConversationsWithUserMessagesAsync();
        Task<IReadOnlyList<MessageDto>> GetMessagesForConversationAsync(int conversationId);
        Task<ConversationDto?> GetOrCreateConversationForUserAsync(int userId);
    }
}