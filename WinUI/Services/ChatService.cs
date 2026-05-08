using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class ChatService : IChatService
{

    private readonly HttpClient httpClient;

    public ChatService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ConversationDto>> GetAllConversationsAsync()
    {
        var conversations = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{ApiBaseUrl.BASE_URL}/api/chat");
        return conversations ?? [];
    }

    public async Task<ConversationDto?> GetOrCreateConversationForUserAsync(int userId)
    {
        var response = await this.httpClient.PostAsync($"{ApiBaseUrl.BASE_URL}/api/chat/user/{userId}", null);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ConversationDto>();
    }

    public async Task<IReadOnlyList<MessageDto>> GetMessagesForConversationAsync(int conversationId)
    {
        var messages = await this.httpClient.GetFromJsonAsync<List<MessageDto>>($"{ApiBaseUrl.BASE_URL}/api/chat/{conversationId}/messages");
        return messages ?? [];
    }

    public async Task<IReadOnlyList<ConversationDto>> GetConversationsWithMessagesAsync()
    {
        var conversations = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{ApiBaseUrl.BASE_URL}/api/chat/with-messages");
        return conversations ?? [];
    }

    public async Task<IReadOnlyList<ConversationDto>> GetConversationsWithUserMessagesAsync()
    {
        var conversations = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{ApiBaseUrl.BASE_URL}/api/chat/with-user-messages");
        return conversations ?? [];
    }

    public async Task<IReadOnlyList<ConversationDto>> GetConversationsWhereNutritionistRespondedAsync(int nutritionistId)
    {
        var conversations = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{ApiBaseUrl.BASE_URL}/api/chat/nutritionist/{nutritionistId}/responded");
        return conversations ?? [];
    }

    public async Task AddMessageAsync(int conversationId, AddMessageRequestDto request)
    {
        var response = await this.httpClient.PostAsJsonAsync($"{ApiBaseUrl.BASE_URL}/api/chat/{conversationId}/messages", request);
        response.EnsureSuccessStatusCode();
    }
}