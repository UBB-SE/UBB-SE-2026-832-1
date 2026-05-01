using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IChatRepository
{
    Task AddMessageAsync(int conversationId, Guid senderId, string text, bool isNutritionist);
    Task<IEnumerable<Conversation>> GetAllConversationsAsync();
    Task<IEnumerable<Conversation>> GetConversationsWhereNutritionistRespondedAsync(Guid nutritionistId);
    Task<IEnumerable<Conversation>> GetConversationsWithMessagesAsync();
    Task<IEnumerable<Conversation>> GetConversationsWithUserMessagesAsync();
    Task<IEnumerable<Message>> GetMessagesForConversationAsync(int conversationId);
    Task<Conversation> GetOrCreateConversationForUserAsync(Guid userId);
}
