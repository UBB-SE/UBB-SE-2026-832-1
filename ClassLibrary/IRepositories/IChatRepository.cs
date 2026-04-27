using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IChatRepository
{
    Task AddMessageAsync(int conversationId, int senderId, string text, bool isNutritionist, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Conversation>> GetAllConversationsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Conversation>> GetConversationsWhereNutritionistRespondedAsync(int nutritionistId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Conversation>> GetConversationsWithMessagesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Conversation>> GetConversationsWithUserMessagesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Message>> GetMessagesForConversationAsync(int conversationId, CancellationToken cancellationToken = default);

    Task<Conversation> GetOrCreateConversationForUserAsync(int userId, CancellationToken cancellationToken = default);
}
