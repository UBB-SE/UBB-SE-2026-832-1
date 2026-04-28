# ChatRepository Refactoring Notes

## Overview
The `ChatRepository` has been refactored from using ADO.NET with direct SQL queries to using Entity Framework Core for data access. This document outlines all changes made during the refactoring process.

---

## Changes Made

### 1. Model Updates
- Updated `Conversation.cs` and `Message.cs` namespaces from `TeamNut.Models` to `ClassLibrary.Models`
- Changed to file-scoped namespaces for consistency with project conventions
- **Added EF Core Data Annotations** to both models ([Key], [Required], [MaxLength], [ForeignKey])
- **Removed denormalized fields** from models (Username, SenderUsername, SenderRole)
- **Removed UI logic** from models (IsFromCurrentUser, SentAtFormatted)
- **Added navigation properties** for proper EF Core relationships
- **Fixed data type mismatch**: Changed UserId and SenderId from `int` to `Guid` to match User.Id

### 2. Interface Updates
- Moved `IChatRepository` from `TeamNut.Repositories.Interfaces` to `ClassLibrary.IRepositories`
- Updated using statements to reference `ClassLibrary.Models`
- Changed method signatures to use `Guid` instead of `int` for user identifiers:
  - `GetOrCreateConversationForUserAsync(Guid userId)`
  - `GetConversationsWhereNutritionistRespondedAsync(Guid nutritionistId)`
  - `AddMessageAsync(int conversationId, Guid senderId, ...)`

### 3. Repository Refactoring
- **Removed**: Direct SQL queries using `SqliteConnection` and `SqliteCommand`
- **Removed**: `Microsoft.Data.Sqlite` dependency
- **Removed**: `IDbConfig` dependency
- **Added**: `AppDbContext` dependency injection
- **Added**: EF Core LINQ queries using `DbContext`
- Changed from `internal` to `public` class for proper DI registration
- Changed namespace from `TeamNut.Repositories` to `ClassLibrary.Repositories`

### 4. Database Context Updates
- Added `DbSet<Conversation>` to `AppDbContext`
- Added `DbSet<Message>` to `AppDbContext`
- Configured relationships in `OnModelCreating()`:
  - Conversation → User (One-to-Many)
  - Conversation → Messages (One-to-Many)
  - Message → Sender (Many-to-One)

### 5. Service Registration
- Registered `IChatRepository` and `ChatRepository` in `ServiceCollectionExtensions.cs`
- Added as scoped service: `services.AddScoped<IChatRepository, ChatRepository>()`

---

## Non-Repository Logic Removed

### 1. UserSession Dependency
**Original Code:**
```csharp
message.IsFromCurrentUser = UserSession.UserId.HasValue && message.SenderId == UserSession.UserId.Value;
```

**Why Removed:**
- Repositories should only handle data access, not presentation logic
- `IsFromCurrentUser` is a UI/presentation concern that depends on the current user's session
- This creates a tight coupling between the repository and the session management system
- Violates Single Responsibility Principle

**Recommended Implementation:**
Handle this in your service layer, controller, or view model:

```csharp
// In a Service or Controller
public async Task<IEnumerable<MessageDto>> GetMessagesWithCurrentUserFlag(int conversationId, Guid currentUserId)
{
    var messages = await _chatRepository.GetMessagesForConversationAsync(conversationId);
    return messages.Select(m => new MessageDto
    {
        Id = m.Id,
        TextContent = m.TextContent,
        SentAt = m.SentAt,
        SenderName = m.Sender?.FullName ?? "Unknown",
        IsFromCurrentUser = m.SenderId == currentUserId // Calculate here
    });
}
```

### 2. Manual SQL Query Construction
**Original Approach:**
```csharp
const string sql = @"
    SELECT c.id, c.has_unanswered, c.user_id, u.username 
    FROM Conversations c 
    JOIN Users u ON c.user_id = u.id 
    ORDER BY c.has_unanswered DESC, c.id DESC";
```

**Why Removed:**
- Manual SQL is error-prone and harder to maintain
- Doesn't leverage EF Core's query optimization
- Makes unit testing more difficult
- Doesn't provide compile-time type safety

**New EF Core Approach:**
```csharp
return await context.Conversations
    .Include(c => c.User)
    .OrderByDescending(c => c.HasUnanswered)
    .ThenByDescending(c => c.Id)
    .ToListAsync();
```

### 3. Manual Data Mapping
**Original Code:**
```csharp
private Conversation MapReaderToConversation(SqliteDataReader reader)
{
    return new Conversation
    {
        Id = reader.GetInt32(0),
        HasUnanswered = Convert.ToBoolean(reader.GetValue(1)),
        UserId = reader.GetInt32(2),
        Username = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
    };
}
```

**Why Removed:**
- EF Core handles object materialization automatically
- Manual mapping is fragile (depends on column order)
- Requires additional code for null checking

**EF Core Handles This:**
EF Core automatically maps database columns to object properties using the entity configuration.

---

## Method-by-Method Changes

### GetAllConversationsAsync()
**Before (ADO.NET):**
```csharp
const string sql = @"
    SELECT c.id, c.has_unanswered, c.user_id, u.username 
    FROM Conversations c 
    JOIN Users u ON c.user_id = u.id 
    ORDER BY c.has_unanswered DESC, c.id DESC";

return await ExecuteConversationQueryAsync(sql);
```

**After (EF Core):**
```csharp
return await context.Conversations
    .Include(c => c.User)
    .OrderByDescending(c => c.HasUnanswered)
    .ThenByDescending(c => c.Id)
    .ToListAsync();
```

**Benefits:**
- Type-safe queries
- Automatic change tracking
- Easy to modify and maintain

---

### GetConversationsWithUserMessagesAsync()
**Before (ADO.NET):**
```csharp
const string sql = @"
    SELECT DISTINCT c.id, c.has_unanswered, c.user_id, u.username 
    FROM Conversations c 
    JOIN Users u ON c.user_id = u.id 
    JOIN Messages m ON m.conversation_id = c.id 
    JOIN Users su ON m.sender_id = su.id 
    WHERE su.role <> 'Nutritionist' 
    ORDER BY c.has_unanswered DESC, c.id DESC";
```

**After (EF Core):**
```csharp
return await context.Conversations
    .Include(c => c.User)
    .Where(c => context.Messages
        .Include(m => m.Sender)
        .Any(m => m.ConversationId == c.Id && m.Sender!.Email != null))
    .OrderByDescending(c => c.HasUnanswered)
    .ThenByDescending(c => c.Id)
    .Distinct()
    .ToListAsync();
```

**Note:** The role filtering logic needs to be updated once the User model includes a Role property.

---

### GetConversationsWhereNutritionistRespondedAsync()
**Before (ADO.NET):**
```csharp
const string sql = @"
    SELECT DISTINCT c.id, c.has_unanswered, c.user_id, u.username 
    FROM Conversations c 
    JOIN Users u ON c.user_id = u.id 
    JOIN Messages m ON m.conversation_id = c.id 
    WHERE m.sender_id = @nid 
    ORDER BY c.has_unanswered DESC, c.id DESC";

var list = new List<Conversation>();
using var conn = new SqliteConnection(connectionString);
using var command = new SqliteCommand(sql, conn);
command.Parameters.AddWithValue("@nid", nutritionistId);
// ... manual reading and mapping
```

**After (EF Core):**
```csharp
return await context.Conversations
    .Include(c => c.User)
    .Where(c => context.Messages
        .Any(m => m.ConversationId == c.Id && m.SenderId == nutritionistId))
    .OrderByDescending(c => c.HasUnanswered)
    .ThenByDescending(c => c.Id)
    .Distinct()
    .ToListAsync();
```

**Benefits:**
- No SQL injection risk
- Parameter handling is automatic
- Type-safe parameter passing

---

### GetOrCreateConversationForUserAsync()
**Before (ADO.NET):**
```csharp
using var conn = new SqliteConnection(connectionString);
await conn.OpenAsync();

const string checkSql = "SELECT id, has_unanswered FROM Conversations WHERE user_id = @uid";
using (var checkCmd = new SqliteCommand(checkSql, conn))
{
    checkCmd.Parameters.AddWithValue("@uid", userId);
    using (var reader = await checkCmd.ExecuteReaderAsync())
    {
        if (await reader.ReadAsync())
        {
            return new Conversation { /* ... */ };
        }
    }
}

const string insertSql = @"
    INSERT INTO Conversations (user_id, has_unanswered) 
    VALUES (@uid, 0); 
    SELECT last_insert_rowid();";
// ... more SQL code
```

**After (EF Core):**
```csharp
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
```

**Benefits:**
- Cleaner, more readable code
- No manual connection management
- Automatic transaction handling
- Database-agnostic (no SQLite-specific `last_insert_rowid()`)

---

### GetMessagesForConversationAsync()
**Before (ADO.NET):**
```csharp
const string sql = @"
    SELECT m.id, m.sent_at, m.conversation_id, m.sender_id, m.text_content, u.username, u.role 
    FROM Messages m 
    JOIN Users u ON m.sender_id = u.id 
    WHERE m.conversation_id = @cid 
    ORDER BY m.sent_at";

var list = new List<Message>();
using var conn = new SqliteConnection(connectionString);
using var command = new SqliteCommand(sql, conn);
command.Parameters.AddWithValue("@cid", conversationId);

await conn.OpenAsync();
using var reader = await command.ExecuteReaderAsync();

while (await reader.ReadAsync())
{
    var message = new Message
    {
        Id = reader.GetInt32(0),
        SentAt = reader.GetDateTime(1),
        // ... manual mapping
        SenderUsername = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
        SenderRole = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
    };
    message.IsFromCurrentUser = UserSession.UserId.HasValue && message.SenderId == UserSession.UserId.Value;
    list.Add(message);
}
```

**After (EF Core):**
```csharp
return await context.Messages
    .Include(m => m.Sender)
    .Where(m => m.ConversationId == conversationId)
    .OrderBy(m => m.SentAt)
    .ToListAsync();
```

**Benefits:**
- Navigation property automatically loads sender information
- No manual null checking required
- UI logic (IsFromCurrentUser) removed from repository
- Denormalized fields (SenderUsername, SenderRole) removed

---

### AddMessageAsync()
**Before (ADO.NET):**
```csharp
using var conn = new SqliteConnection(connectionString);
await conn.OpenAsync();

const string insertSql = @"
    INSERT INTO Messages (conversation_id, sender_id, text_content) 
    VALUES (@cid, @sid, @txt)";

using (var command = new SqliteCommand(insertSql, conn))
{
    command.Parameters.AddWithValue("@cid", conversationId);
    command.Parameters.AddWithValue("@sid", senderId);
    command.Parameters.AddWithValue("@txt", text);
    await command.ExecuteNonQueryAsync();
}

const string updateSql = @"
    UPDATE Conversations 
    SET has_unanswered = CASE WHEN @isUser = 1 THEN 1 ELSE 0 END 
    WHERE id = @cid";

using (var updateCmd = new SqliteCommand(updateSql, conn))
{
    updateCmd.Parameters.AddWithValue("@isUser", isNutritionist ? 0 : 1);
    updateCmd.Parameters.AddWithValue("@cid", conversationId);
    await updateCmd.ExecuteNonQueryAsync();
}
```

**After (EF Core):**
```csharp
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
```

**Benefits:**
- Single `SaveChangesAsync()` call for both operations (transaction)
- Change tracking handles the update automatically
- Cleaner, more maintainable code
- DateTime assignment is explicit

---

## Database Relationship Configuration

### Relationships Added to AppDbContext.OnModelCreating()

```csharp
// Conversation → User (Many-to-One)
modelBuilder.Entity<Conversation>()
    .HasOne(c => c.User)
    .WithMany()
    .HasForeignKey(c => c.UserId)
    .OnDelete(DeleteBehavior.Cascade);

// Conversation → Messages (One-to-Many)
modelBuilder.Entity<Conversation>()
    .HasMany(c => c.Messages)
    .WithOne(m => m.Conversation)
    .HasForeignKey(m => m.ConversationId)
    .OnDelete(DeleteBehavior.Cascade);

// Message → Sender (Many-to-One)
modelBuilder.Entity<Message>()
    .HasOne(m => m.Sender)
    .WithMany()
    .HasForeignKey(m => m.SenderId)
    .OnDelete(DeleteBehavior.Restrict);
```

### Delete Behaviors Explained
- **Cascade (Conversation → Messages)**: When a conversation is deleted, all its messages are automatically deleted
- **Cascade (Conversation → User)**: When a user is deleted, all their conversations are deleted
- **Restrict (Message → Sender)**: Cannot delete a user if they have sent messages (prevents orphaned messages)

---

## Service Registration

### Updated ServiceCollectionExtensions.cs

```csharp
public static IServiceCollection AddClassLibraryDataAccess(this IServiceCollection services)
{
    services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase(IN_MEMORY_DATABASE_NAME));
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IRepositoryAchievements, RepositoryAchievements>();
    services.AddScoped<IRepositoryNotification, RepositoryNotification>();
    services.AddScoped<IRepositoryNutrition, RepositoryNutrition>();
    services.AddScoped<IChatRepository, ChatRepository>(); // ← Added
    
    return services;
}
```

---

## Benefits of This Refactoring

### 1. **Maintainability**
- LINQ queries are easier to read and modify than raw SQL
- Type-safe queries prevent many runtime errors
- Changes to models automatically reflect in queries

### 2. **Testability**
- Can easily mock `AppDbContext` for unit testing
- No need to mock `SqliteConnection` or `SqliteCommand`
- Can use in-memory database for testing

### 3. **Database Agnostic**
- Not tied to SQLite anymore
- Can easily switch to SQL Server, PostgreSQL, etc.
- Just change the connection string and provider

### 4. **Security**
- No SQL injection risks with parameterized LINQ queries
- EF Core handles parameter sanitization automatically

### 5. **Performance**
- EF Core optimizes queries automatically
- Change tracking reduces unnecessary database calls
- Can use `AsNoTracking()` for read-only queries if needed

### 6. **Separation of Concerns**
- Repository only handles data access
- UI/presentation logic moved to appropriate layers
- Business logic can be handled in service layer

---

## Known Issues & Future Improvements

### 1. Role-Based Filtering
The `GetConversationsWithUserMessagesAsync()` method currently uses a placeholder filter:
```csharp
.Any(m => m.ConversationId == c.Id && m.Sender!.Email != null)
```

**Action Required:**
- Add a `Role` property to the `User` model
- Update the filter to: `m.Sender!.Role != "Nutritionist"`

### 2. Username Access
Since `Username` was removed from `Conversation`, consumers must now use:
```csharp
conversation.User?.FullName
// or
conversation.User?.Email
```

### 3. Navigation Properties Loading
Some methods use `.Include()` to eagerly load related entities. Consider:
- Using explicit loading for specific scenarios
- Using `AsNoTracking()` for read-only queries
- Implementing projection (Select) for better performance

Example:
```csharp
// Instead of loading full entities
return await context.Conversations
    .Include(c => c.User)
    .Select(c => new ConversationDto
    {
        Id = c.Id,
        HasUnanswered = c.HasUnanswered,
        Username = c.User!.FullName
    })
    .ToListAsync();
```

---

## Testing Recommendations

### Unit Testing
```csharp
[Fact]
public async Task GetMessagesForConversationAsync_ReturnsMessagesWithSender()
{
    // Arrange
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDb")
        .Options;

    using var context = new AppDbContext(options);
    var repository = new ChatRepository(context);
    
    // Add test data...
    
    // Act
    var messages = await repository.GetMessagesForConversationAsync(1);
    
    // Assert
    Assert.NotEmpty(messages);
    Assert.All(messages, m => Assert.NotNull(m.Sender));
}
```

### Integration Testing
- Test with actual database (not in-memory)
- Verify cascade deletes work correctly
- Test concurrent updates to conversations

---

## Migration Checklist

- [x] Update model namespaces
- [x] Add data annotations to models
- [x] Remove denormalized fields from models
- [x] Remove UI logic from models
- [x] Add navigation properties
- [x] Update `AppDbContext` with DbSets
- [x] Configure relationships in `OnModelCreating()`
- [x] Refactor repository to use EF Core
- [x] Update interface signatures
- [x] Register repository in DI container
- [ ] Update all consumers to use navigation properties
- [ ] Add `Role` property to User model (if needed)
- [ ] Create ViewModels for presentation logic
- [ ] Update UI components
- [ ] Write unit tests
- [ ] Write integration tests
- [ ] Update documentation

---

## Questions or Issues?

If you encounter any problems during the migration or have questions about the refactoring, please contact the development team.

**Related Documentation:**
- See `CHAT_MODELS_REFACTORING_REPORT.md` for detailed information about model changes
- See EF Core documentation: https://learn.microsoft.com/en-us/ef/core/
