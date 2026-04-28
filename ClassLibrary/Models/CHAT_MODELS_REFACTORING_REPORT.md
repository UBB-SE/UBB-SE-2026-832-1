# Chat Models Refactoring Report

## Overview
Both `Conversation.cs` and `Message.cs` models have been refactored to comply with EF Core best practices and data annotation requirements.

---

## ✅ Conversation Model (`Conversation.cs`)

### Applied Data Annotations
- `[Key]` on `Id` property
- `[Required]` on `HasUnanswered` property
- `[Required]` on `UserId` property
- `[ForeignKey(nameof(User))]` on `UserId` property

### Navigation Properties Added
- `public virtual User? User { get; set; }` - Links to the User entity
- `public virtual ICollection<Message> Messages { get; set; }` - Collection of messages in this conversation

### Business/UI Logic Removed

#### ❌ Removed: Username Property (Denormalized Data)
**Original Code:**
```csharp
public string Username { get; set; } = string.Empty;
```

**Why Removed:**
- This is denormalized data that should come from the `User` navigation property
- Violates database normalization principles
- Creates data redundancy and potential inconsistencies

**How to Access Now:**
```csharp
// Instead of: conversation.Username
// Use: conversation.User?.FullName or conversation.User?.Email
var conversation = await chatRepository.GetAllConversationsAsync();
var username = conversation.User?.FullName;
```

### Data Type Corrections
- Changed `UserId` from `int` to `Guid` to match the `User.Id` type

---

## ✅ Message Model (`Message.cs`)

### Applied Data Annotations
- `[Key]` on `Id` property
- `[Required]` on `SentAt` property
- `[Required]` on `ConversationId` property
- `[Required]` on `SenderId` property
- `[Required]` on `TextContent` property
- `[MaxLength(5000)]` on `TextContent` property (limits message size)
- `[ForeignKey(nameof(Conversation))]` on `ConversationId` property
- `[ForeignKey(nameof(Sender))]` on `SenderId` property

### Navigation Properties Added
- `public virtual Conversation? Conversation { get; set; }` - Links to the parent conversation
- `public virtual User? Sender { get; set; }` - Links to the user who sent the message

### Business/UI Logic Removed

#### ❌ Removed: SenderUsername Property (Denormalized Data)
**Original Code:**
```csharp
public string SenderUsername { get; set; } = string.Empty;
```

**Why Removed:**
- Denormalized data from the User entity
- Should be accessed via `Sender` navigation property

**How to Access Now:**
```csharp
// Instead of: message.SenderUsername
// Use: message.Sender?.FullName or message.Sender?.Email
var messages = await chatRepository.GetMessagesForConversationAsync(conversationId);
var senderName = messages.First().Sender?.FullName;
```

#### ❌ Removed: SenderRole Property (Denormalized Data)
**Original Code:**
```csharp
public string SenderRole { get; set; } = string.Empty;
```

**Why Removed:**
- This property doesn't exist in the current `User` model
- Appears to be legacy code from a previous schema
- If roles are needed, they should be added to the `User` model or a separate `Role` entity

**Recommendation:**
If you need role-based functionality, consider:
1. Adding a `Role` property to the `User` model
2. Creating a separate `Role` entity with a many-to-many relationship
3. Using ASP.NET Core Identity for role management

#### ❌ Removed: IsFromCurrentUser Property (UI Logic)
**Original Code:**
```csharp
public bool IsFromCurrentUser { get; set; }
```

**Why Removed:**
- This is presentation layer logic, not data
- Models should represent database entities, not UI state
- The "current user" concept doesn't exist in the data layer

**How to Implement Now:**
Handle this in your ViewModel or Presentation Layer:
```csharp
// In your ViewModel or Controller
var messages = await chatRepository.GetMessagesForConversationAsync(conversationId);
var messageViewModels = messages.Select(m => new MessageViewModel
{
    Id = m.Id,
    TextContent = m.TextContent,
    SentAt = m.SentAt,
    SenderName = m.Sender?.FullName ?? "Unknown",
    IsFromCurrentUser = m.SenderId == currentUserId // Calculate here
}).ToList();
```

#### ❌ Removed: SentAtFormatted Property (UI Formatting)
**Original Code:**
```csharp
public string SentAtFormatted => SentAt.ToString("g");
```

**Why Removed:**
- Formatting logic belongs in the presentation layer
- Date/time formatting may vary by user locale
- Models should contain raw data, not formatted strings

**How to Format Now:**
Handle formatting in your View or ViewModel:
```csharp
// In Razor View:
@message.SentAt.ToString("g")

// Or in ViewModel:
public class MessageViewModel
{
    public DateTime SentAt { get; set; }
    public string SentAtFormatted => SentAt.ToString("g");
}
```

### Data Type Corrections
- Changed `SenderId` from `int` to `Guid` to match the `User.Id` type

---

## Database Context Updates (`AppDbContext.cs`)

### Relationship Configurations Added
```csharp
// Conversation -> User relationship
modelBuilder.Entity<Conversation>()
    .HasOne(c => c.User)
    .WithMany()
    .HasForeignKey(c => c.UserId)
    .OnDelete(DeleteBehavior.Cascade);

// Conversation -> Messages relationship
modelBuilder.Entity<Conversation>()
    .HasMany(c => c.Messages)
    .WithOne(m => m.Conversation)
    .HasForeignKey(m => m.ConversationId)
    .OnDelete(DeleteBehavior.Cascade);

// Message -> Sender (User) relationship
modelBuilder.Entity<Message>()
    .HasOne(m => m.Sender)
    .WithMany()
    .HasForeignKey(m => m.SenderId)
    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
```

### Delete Behaviors
- **Cascade Delete**: When a Conversation is deleted, all its Messages are deleted
- **Restrict Delete**: Cannot delete a User if they have sent messages (prevents orphaned messages)

---

## Repository Updates (`ChatRepository.cs`)

### Changes Made
1. Added `.Include()` calls to load related entities:
   - `GetAllConversationsAsync()` now includes `User`
   - `GetMessagesForConversationAsync()` now includes `Sender`
   
2. Updated method signatures to use `Guid` instead of `int`:
   - `GetOrCreateConversationForUserAsync(Guid userId)`
   - `GetConversationsWhereNutritionistRespondedAsync(Guid nutritionistId)`
   - `AddMessageAsync(int conversationId, Guid senderId, ...)`

3. Removed `SenderRole` filtering:
   - `GetConversationsWithUserMessagesAsync()` query needs to be updated based on your role implementation

---

## Migration Guide for Consumers

### For Controllers/Services Using ChatRepository

#### Before:
```csharp
var messages = await _chatRepository.GetMessagesForConversationAsync(conversationId);
foreach (var message in messages)
{
    Console.WriteLine($"{message.SenderUsername}: {message.TextContent}");
    if (message.IsFromCurrentUser)
    {
        // Do something
    }
}
```

#### After:
```csharp
var messages = await _chatRepository.GetMessagesForConversationAsync(conversationId);
foreach (var message in messages)
{
    var senderName = message.Sender?.FullName ?? "Unknown";
    Console.WriteLine($"{senderName}: {message.TextContent}");
    
    var currentUserId = /* Get from session/auth */;
    if (message.SenderId == currentUserId)
    {
        // Do something
    }
}
```

### For Views/ViewModels

Create ViewModels for presentation logic:

```csharp
public class MessageViewModel
{
    public int Id { get; set; }
    public string TextContent { get; set; }
    public DateTime SentAt { get; set; }
    public string SenderName { get; set; }
    public string SentAtFormatted { get; set; }
    public bool IsFromCurrentUser { get; set; }
    
    public static MessageViewModel FromModel(Message message, Guid currentUserId)
    {
        return new MessageViewModel
        {
            Id = message.Id,
            TextContent = message.TextContent,
            SentAt = message.SentAt,
            SenderName = message.Sender?.FullName ?? "Unknown",
            SentAtFormatted = message.SentAt.ToString("g"),
            IsFromCurrentUser = message.SenderId == currentUserId
        };
    }
}
```

---

## Action Items for Team

### 1. Update User Model (If Needed)
If role-based functionality is required, add to `User.cs`:
```csharp
public sealed class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    
    // Add if roles are needed:
    [MaxLength(50)]
    public string Role { get; set; } = "User"; // Default role
}
```

### 2. Update All Controllers/Services
Search for usage of:
- `conversation.Username` → Replace with `conversation.User?.FullName`
- `message.SenderUsername` → Replace with `message.Sender?.FullName`
- `message.SenderRole` → Implement role logic
- `message.IsFromCurrentUser` → Calculate in presentation layer
- `message.SentAtFormatted` → Format in views

### 3. Create ViewModels
Create separate ViewModel classes for presentation logic to keep models clean.

### 4. Update GetConversationsWithUserMessagesAsync
The method currently has a placeholder filter. Update based on your role implementation:
```csharp
// Current placeholder:
.Where(c => context.Messages
    .Include(m => m.Sender)
    .Any(m => m.ConversationId == c.Id && m.Sender!.Email != null))

// Update to something like:
.Where(c => context.Messages
    .Include(m => m.Sender)
    .Any(m => m.ConversationId == c.Id && m.Sender!.Role != "Nutritionist"))
```

---

## Testing Checklist

- [ ] Verify all conversation queries return `User` data via navigation property
- [ ] Verify all message queries return `Sender` data via navigation property
- [ ] Test cascade delete: Deleting a conversation deletes its messages
- [ ] Test restrict delete: Cannot delete a user with messages
- [ ] Update all UI components to use navigation properties instead of denormalized fields
- [ ] Verify date formatting works correctly in all views
- [ ] Test `IsFromCurrentUser` logic in presentation layer
- [ ] Verify role-based filtering works (if implemented)

---

## Summary

✅ **Completed:**
- Added all required data annotations ([Key], [Required], [MaxLength], [ForeignKey])
- Removed all business/UI logic from models
- Added proper navigation properties
- Configured EF Core relationships in DbContext
- Updated repository to use navigation properties
- Fixed data type mismatches (int → Guid)

⚠️ **Action Required:**
- Update all consumers of these models to use navigation properties
- Implement role logic if needed in User model
- Create ViewModels for presentation logic
- Update UI components to handle formatting
- Test thoroughly before deploying

---

**Questions or Issues?**
Contact the development team if you encounter any issues during migration.
