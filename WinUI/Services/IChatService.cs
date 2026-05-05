using ClassLibrary.DTOs;

namespace WinUI.Services
{
    public interface IChatService
    {
        Task AddMessageAsync(int conversationId, AddMessageRequestDto request);
        Task<IReadOnlyList<ConversationDto>> GetAllConversationsAsync();
        Task<IReadOnlyList<ConversationDto>> GetConversationsWhereNutritionistRespondedAsync(Guid nutritionistId);
        Task<IReadOnlyList<ConversationDto>> GetConversationsWithMessagesAsync();
        Task<IReadOnlyList<ConversationDto>> GetConversationsWithUserMessagesAsync();
        Task<IReadOnlyList<MessageDto>> GetMessagesForConversationAsync(int conversationId);
        Task<ConversationDto?> GetOrCreateConversationForUserAsync(Guid userId);
    }
}