using ClassLibrary.Models;

namespace WinUI.Services
{
    public interface IChatService
    {
        Task AddMessageAsync(int conversationId, int senderId, string textContent, bool isNutritionist);
        Task<IReadOnlyList<Conversation>> GetAllConversationsAsync();
        Task<IReadOnlyList<Conversation>> GetConversationsWhereNutritionistRespondedAsync(int nutritionistId);
        Task<IReadOnlyList<Conversation>> GetConversationsWithMessagesAsync();
        Task<IReadOnlyList<Conversation>> GetConversationsWithUserMessagesAsync();
        Task<IReadOnlyList<Message>> GetMessagesForConversationAsync(int conversationId);
        Task<Conversation?> GetOrCreateConversationForUserAsync(int userId);
    }
}