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
        // Note: Since User model doesn't have a Role property, this method returns all conversations with messages.
        // To distinguish between user and nutritionist messages, consider adding an IsFromNutritionist
        // property to the Message model or adding Role property to User model.
        return await context.Conversations
            .Include(c => c.User)
            .Where(c => c.Messages.Any())
            .OrderByDescending(c => c.HasUnanswered)
            .ThenByDescending(c => c.Id)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<Conversation>> GetConversationsWithMessagesAsync()
    {
        return await context.Conversations
            .Include(c => c.User)
            .Where(c => c.Messages.Any())
            .OrderByDescending(c => c.HasUnanswered)
            .ThenByDescending(c => c.Id)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<Conversation>> GetConversationsWhereNutritionistRespondedAsync(Guid nutritionistId)
    {
        return await context.Conversations
            .Include(c => c.User)
            .Where(c => c.Messages.Any(m => EF.Property<Guid>(m.Sender, "Id") == nutritionistId))
            .OrderByDescending(c => c.HasUnanswered)
            .ThenByDescending(c => c.Id)
            .Distinct()
            .ToListAsync();
    }

    public async Task<Conversation> GetOrCreateConversationForUserAsync(Guid userId)
    {
        var conversation = await context.Conversations
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => EF.Property<Guid>(c.User, "Id") == userId);

        if (conversation != null)
        {
            return conversation;
        }

        var user = await context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found.");
        }

        conversation = new Conversation
        {
            User = user,
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
            .Include(m => m.Conversation)
            .Where(m => EF.Property<int>(m.Conversation, "Id") == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task AddMessageAsync(int conversationId, Guid senderId, string text, bool isNutritionist)
    {
        var conversation = await context.Conversations.FindAsync(conversationId);
        if (conversation == null)
        {
            throw new InvalidOperationException($"Conversation with ID {conversationId} not found.");
        }

        var sender = await context.Users.FindAsync(senderId);
        if (sender == null)
        {
            throw new InvalidOperationException($"User with ID {senderId} not found.");
        }

        var message = new Message
        {
            Conversation = conversation,
            Sender = sender,
            TextContent = text,
            SentAt = DateTime.UtcNow
        };

        context.Messages.Add(message);
        conversation.HasUnanswered = !isNutritionist;

        await context.SaveChangesAsync();
    }
}
