using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

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
            .Include(conversation => conversation.User)
            .OrderByDescending(conversation => conversation.HasUnanswered)
            .ThenByDescending(conversation => conversation.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Conversation>> GetConversationsWithUserMessagesAsync()
    {
        return await context.Conversations
            .Include(conversation => conversation.User)
            .Where(conversation => conversation.Messages.Any())
            .OrderByDescending(conversation => conversation.HasUnanswered)
            .ThenByDescending(conversation => conversation.Id)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<Conversation>> GetConversationsWithMessagesAsync()
    {
        return await context.Conversations
            .Include(conversation => conversation.User)
            .Where(conversation => conversation.Messages.Any())
            .OrderByDescending(conversation => conversation.HasUnanswered)
            .ThenByDescending(conversation => conversation.Id)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<Conversation>> GetConversationsWhereNutritionistRespondedAsync(Guid nutritionistId)
    {
        return await context.Conversations
            .Include(conversation => conversation.User)
            .Where(conversation => conversation.Messages.Any(message => EF.Property<Guid>(message.Sender, "Id") == nutritionistId))
            .OrderByDescending(conversation => conversation.HasUnanswered)
            .ThenByDescending(conversation => conversation.Id)
            .Distinct()
            .ToListAsync();
    }

    public async Task<Conversation> GetOrCreateConversationForUserAsync(Guid userId)
    {
        var conversation = await context.Conversations
            .Include(conversationItem => conversationItem.User)
            .FirstOrDefaultAsync(conversationItem => EF.Property<Guid>(conversationItem.User, "Id") == userId);

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
            .Include(message => message.Sender)
            .Include(message => message.Conversation)
            .Where(message => EF.Property<int>(message.Conversation, "Id") == conversationId)
            .OrderBy(message => message.SentAt)
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
