using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.Proxies.Interfaces;

namespace ClassLibrary.Proxies;

public sealed class ChatProxy : IChatProxy
{
    private readonly HttpClient httpClient;

    public ChatProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<Conversation>> GetAllConversationsAsync()
    {
        var dtos = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{ApiBaseUrl.BASE_URL}/api/chat");
        return DataTransferObjectToDomainModelMappers.MapConversations(dtos);
    }

    public async Task<Conversation?> GetOrCreateConversationForUserAsync(int userId)
    {
        var response = await this.httpClient.PostAsync($"{ApiBaseUrl.BASE_URL}/api/chat/user/{userId}", null);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<ConversationDto>();
        return dto != null ? DataTransferObjectToDomainModelMappers.MapConversation(dto) : null;
    }

    public async Task<IReadOnlyList<Message>> GetMessagesForConversationAsync(int conversationId)
    {
        var dtos = await this.httpClient.GetFromJsonAsync<List<MessageDto>>($"{ApiBaseUrl.BASE_URL}/api/chat/{conversationId}/messages");
        return DataTransferObjectToDomainModelMappers.MapMessages(dtos);
    }

    public async Task<IReadOnlyList<Conversation>> GetConversationsWithMessagesAsync()
    {
        var dtos = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{ApiBaseUrl.BASE_URL}/api/chat/with-messages");
        return DataTransferObjectToDomainModelMappers.MapConversations(dtos);
    }

    public async Task<IReadOnlyList<Conversation>> GetConversationsWithUserMessagesAsync()
    {
        var dtos = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{ApiBaseUrl.BASE_URL}/api/chat/with-user-messages");
        return DataTransferObjectToDomainModelMappers.MapConversations(dtos);
    }

    public async Task<IReadOnlyList<Conversation>> GetConversationsWhereNutritionistRespondedAsync(int nutritionistId)
    {
        var dtos = await this.httpClient.GetFromJsonAsync<List<ConversationDto>>($"{ApiBaseUrl.BASE_URL}/api/chat/nutritionist/{nutritionistId}/responded");
        return DataTransferObjectToDomainModelMappers.MapConversations(dtos);
    }

    public async Task AddMessageAsync(int conversationId, int senderId, string textContent, bool isNutritionist)
    {
        var request = new AddMessageRequestDto
        {
            SenderId = senderId,
            Text = textContent,
            IsNutritionist = isNutritionist
        };

        var response = await this.httpClient.PostAsJsonAsync($"{ApiBaseUrl.BASE_URL}/api/chat/{conversationId}/messages", request);
        response.EnsureSuccessStatusCode();
    }
}


