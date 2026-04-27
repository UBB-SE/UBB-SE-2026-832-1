using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class ChatRepository(AppDbContext dbContext) : IChatRepository
{
    public async Task<IReadOnlyList<Conversation>> GetAllConversationsAsync(CancellationToken cancellationToken = default)
    {
        var conversations = await dbContext.Conversations
            .AsNoTracking()
            .OrderByDescending(c => c.HasUnanswered)
            .ThenByDescending(c => c.Id)
            .ToListAsync(cancellationToken);

        foreach (var conversation in conversations)
        {
            var user = await dbContext.NutUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == conversation.UserId, cancellationToken);

            conversation.Username = user?.Username ?? string.Empty;
        }

        return conversations;
    }

    public async Task<IReadOnlyList<Conversation>> GetConversationsWithUserMessagesAsync(CancellationToken cancellationToken = default)
    {
        var conversationIds = await dbContext.Messages
            .AsNoTracking()
            .Where(m => m.SenderRole != "Nutritionist")
            .Select(m => m.ConversationId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var conversations = await dbContext.Conversations
            .AsNoTracking()
            .Where(c => conversationIds.Contains(c.Id))
            .OrderByDescending(c => c.HasUnanswered)
            .ThenByDescending(c => c.Id)
            .ToListAsync(cancellationToken);

        foreach (var conversation in conversations)
        {
            var user = await dbContext.NutUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == conversation.UserId, cancellationToken);

            conversation.Username = user?.Username ?? string.Empty;
        }

        return conversations;
    }

    public async Task<IReadOnlyList<Conversation>> GetConversationsWithMessagesAsync(CancellationToken cancellationToken = default)
    {
        var conversationIds = await dbContext.Messages
            .AsNoTracking()
            .Select(m => m.ConversationId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var conversations = await dbContext.Conversations
            .AsNoTracking()
            .Where(c => conversationIds.Contains(c.Id))
            .OrderByDescending(c => c.HasUnanswered)
            .ThenByDescending(c => c.Id)
            .ToListAsync(cancellationToken);

        foreach (var conversation in conversations)
        {
            var user = await dbContext.NutUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == conversation.UserId, cancellationToken);

            conversation.Username = user?.Username ?? string.Empty;
        }

        return conversations;
    }

    public async Task<IReadOnlyList<Conversation>> GetConversationsWhereNutritionistRespondedAsync(int nutritionistId, CancellationToken cancellationToken = default)
    {
        var conversationIds = await dbContext.Messages
            .AsNoTracking()
            .Where(m => m.SenderId == nutritionistId)
            .Select(m => m.ConversationId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var conversations = await dbContext.Conversations
            .AsNoTracking()
            .Where(c => conversationIds.Contains(c.Id))
            .OrderByDescending(c => c.HasUnanswered)
            .ThenByDescending(c => c.Id)
            .ToListAsync(cancellationToken);

        foreach (var conversation in conversations)
        {
            var user = await dbContext.NutUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == conversation.UserId, cancellationToken);

            conversation.Username = user?.Username ?? string.Empty;
        }

        return conversations;
    }

    public async Task<Conversation> GetOrCreateConversationForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Conversations
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (existing is not null)
        {
            return existing;
        }

        var conversation = new Conversation
        {
            UserId = userId,
            HasUnanswered = false
        };

        dbContext.Conversations.Add(conversation);
        await dbContext.SaveChangesAsync(cancellationToken);

        return conversation;
    }

    public async Task<IReadOnlyList<Message>> GetMessagesForConversationAsync(int conversationId, CancellationToken cancellationToken = default)
    {
        var messages = await dbContext.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync(cancellationToken);

        var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();
        var senders = await dbContext.NutUsers
            .AsNoTracking()
            .Where(u => senderIds.Contains(u.Id))
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            var sender = senders.FirstOrDefault(s => s.Id == message.SenderId);
            message.SenderUsername = sender?.Username ?? string.Empty;
            message.SenderRole = sender?.Role ?? string.Empty;
        }

        return messages;
    }

    public async Task AddMessageAsync(int conversationId, int senderId, string text, bool isNutritionist, CancellationToken cancellationToken = default)
    {
        dbContext.Messages.Add(new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            TextContent = text,
            SentAt = System.DateTime.Now
        });

        var conversation = await dbContext.Conversations
            .FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken);

        if (conversation is not null)
        {
            conversation.HasUnanswered = !isNutritionist;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
