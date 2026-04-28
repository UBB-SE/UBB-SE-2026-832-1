namespace ClassLibrary.Repositories;

using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

public class ChatRepository : IChatRepository
{
    private readonly AppDbContext context;

    public ChatRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<Conversation>> GetAllConversationsAsync()
    {
        return await context.Conversations
            .Include(c => c.User)
            .OrderByDescending(c => c.HasUnanswered)
            .ThenByDescending(c => c.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Conversation>> GetConversationsWithUserMessagesAsync()
    {
        return await context.Conversations
            .Include(c => c.User)
            .Where(c => context.Messages
                .Include(m => m.Sender)
                .Any(m => m.ConversationId == c.Id && m.Sender!.Email != null))
            .OrderByDescending(c => c.HasUnanswered)
            .ThenByDescending(c => c.Id)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<Conversation>> GetConversationsWithMessagesAsync()
    {
        return await context.Conversations
            .Include(c => c.User)
            .Where(c => context.Messages.Any(m => m.ConversationId == c.Id))
            .OrderByDescending(c => c.HasUnanswered)
            .ThenByDescending(c => c.Id)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<Conversation>> GetConversationsWhereNutritionistRespondedAsync(Guid nutritionistId)
    {
        return await context.Conversations
            .Include(c => c.User)
            .Where(c => context.Messages
                .Any(m => m.ConversationId == c.Id && m.SenderId == nutritionistId))
            .OrderByDescending(c => c.HasUnanswered)
            .ThenByDescending(c => c.Id)
            .Distinct()
            .ToListAsync();
    }

    public async Task<Conversation> GetOrCreateConversationForUserAsync(Guid userId)
    {
        var conversation = await context.Conversations
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (conversation != null)
        {
            return conversation;
        }

        conversation = new Conversation
        {
            UserId = userId,
            HasUnanswered = false
        };

        context.Conversations.Add(conversation);
        await context.SaveChangesAsync();

        return conversation;
    }

    public async Task<IEnumerable<Message>> GetMessagesForConversationAsync(int conversationId)
    {
        return await context.Messages
            .Include(m => m.Sender)
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task AddMessageAsync(int conversationId, Guid senderId, string text, bool isNutritionist)
    {
        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            TextContent = text,
            SentAt = DateTime.UtcNow
        };

        context.Messages.Add(message);

        var conversation = await context.Conversations.FindAsync(conversationId);
        if (conversation != null)
        {
            conversation.HasUnanswered = !isNutritionist;
        }

        await context.SaveChangesAsync();
    }
}
