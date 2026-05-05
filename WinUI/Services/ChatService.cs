using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class ChatService : IChatService
{
    private const string API_BASE_ADDRESS = "https://localhost:7197";

    private readonly HttpClient httpClient;

    public ChatService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ConversationDto>> GetAllConversationsAsync()
    {
        var conversations = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{API_BASE_ADDRESS}/api/chat");
        return conversations ?? [];
    }

    public async Task<ConversationDto?> GetOrCreateConversationForUserAsync(Guid userId)
    {
        var response = await this.httpClient.PostAsync($"{API_BASE_ADDRESS}/api/chat/user/{userId}", null);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ConversationDto>();
    }

    public async Task<IReadOnlyList<MessageDto>> GetMessagesForConversationAsync(int conversationId)
    {
        var messages = await this.httpClient.GetFromJsonAsync<List<MessageDto>>($"{API_BASE_ADDRESS}/api/chat/{conversationId}/messages");
        return messages ?? [];
    }

    public async Task<IReadOnlyList<ConversationDto>> GetConversationsWithMessagesAsync()
    {
        var conversations = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{API_BASE_ADDRESS}/api/chat/with-messages");
        return conversations ?? [];
    }

    public async Task<IReadOnlyList<ConversationDto>> GetConversationsWithUserMessagesAsync()
    {
        var conversations = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{API_BASE_ADDRESS}/api/chat/with-user-messages");
        return conversations ?? [];
    }

    public async Task<IReadOnlyList<ConversationDto>> GetConversationsWhereNutritionistRespondedAsync(Guid nutritionistId)
    {
        var conversations = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{API_BASE_ADDRESS}/api/chat/nutritionist/{nutritionistId}/responded");
        return conversations ?? [];
    }

    public async Task AddMessageAsync(int conversationId, AddMessageRequestDto request)
    {
        var response = await this.httpClient.PostAsJsonAsync($"{API_BASE_ADDRESS}/api/chat/{conversationId}/messages", request);
        response.EnsureSuccessStatusCode();
    }
}