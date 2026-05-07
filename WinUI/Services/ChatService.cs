using ClassLibrary.DTOs;
using ClassLibrary.Models;
using System.Net.Http.Json;

namespace WinUI.Services;

public sealed class ChatService : IChatService
{
    private const string API_BASE_ADDRESS = "https://localhost:7197";

    private readonly HttpClient httpClient;

    public ChatService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    private static Guid MapIntToGuid(int id)
    {
        return new Guid(id.ToString("D32"));
    }

    private static Conversation MapToConversation(ConversationDto dto)
    {
        return new Conversation
        {
            Id = dto.ConversationId,
            HasUnanswered = dto.HasUnanswered,

            User = new User
            {
                UserId = dto.UserId,
                Username = dto.UserName
            },

            Messages = new List<Message>()
        };
    }

    private static Message MapToMessage(MessageDto dto)
    {
        return new Message
        {
            Id = dto.MessageId,
            SentAt = dto.SentAt,
            TextContent = dto.TextContent,

            Sender = new User
            {
                Username = dto.SenderUsername,
                Role = dto.SenderRole
            },

            Conversation = new Conversation
            {
                Id = dto.ConversationId
            }
        };
    }

    public async Task<IReadOnlyList<Conversation>> GetAllConversationsAsync()
    {
        var dtos = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{API_BASE_ADDRESS}/api/chat");

        if (dtos is null) return [];

        return dtos.Select(MapToConversation).ToList();
    }

    public async Task<Conversation?> GetOrCreateConversationForUserAsync(int userId)
    {
        var userGuid = MapIntToGuid(userId);
        var response = await this.httpClient.PostAsync($"{API_BASE_ADDRESS}/api/chat/user/{userGuid}", null);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<ConversationDto>();
        return dto is null ? null : MapToConversation(dto);
    }

    public async Task<IReadOnlyList<Message>> GetMessagesForConversationAsync(int conversationId)
    {
        var dtos = await this.httpClient.GetFromJsonAsync<List<MessageDto>>($"{API_BASE_ADDRESS}/api/chat/{conversationId}/messages");

        if (dtos is null) return [];

        return dtos.Select(MapToMessage).ToList();
    }

    public async Task<IReadOnlyList<Conversation>> GetConversationsWithMessagesAsync()
    {
        var dtos = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{API_BASE_ADDRESS}/api/chat/with-messages");

        if (dtos is null) return [];

        return dtos.Select(MapToConversation).ToList();
    }

    public async Task<IReadOnlyList<Conversation>> GetConversationsWithUserMessagesAsync()
    {
        var dtos = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{API_BASE_ADDRESS}/api/chat/with-user-messages");

        if (dtos is null) return [];

        return dtos.Select(MapToConversation).ToList();
    }

    public async Task<IReadOnlyList<Conversation>> GetConversationsWhereNutritionistRespondedAsync(int nutritionistId)
    {
        var nutritionistGuid = MapIntToGuid(nutritionistId);
        var dtos = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{API_BASE_ADDRESS}/api/chat/nutritionist/{nutritionistGuid}/responded");

        if (dtos is null) return [];

        return dtos.Select(MapToConversation).ToList();
    }

    public async Task AddMessageAsync(int conversationId, int senderId, string text, bool isNutritionist)
    {
        var request = new AddMessageRequestDto
        {
            SenderId = MapIntToGuid(senderId),
            Text = text,
            IsNutritionist = isNutritionist
        };

        var response = await this.httpClient.PostAsJsonAsync($"{API_BASE_ADDRESS}/api/chat/{conversationId}/messages", request);
        response.EnsureSuccessStatusCode();
    }
}