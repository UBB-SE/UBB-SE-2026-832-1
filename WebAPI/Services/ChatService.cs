using ClassLibrary.IRepositories;
using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WebApi.Services
{
    public class ChatService
    {
        private readonly IChatRepository chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        public async Task<IEnumerable<ConversationDto>> GetAllConversationsAsync()
        {
            var conversations = await this.chatRepository.GetAllConversationsAsync();

            if (conversations == null)
            {
                return Enumerable.Empty<ConversationDto>();
            }

            return MapToConversationDtos(conversations);
        }

        public async Task<ConversationDto> GetOrCreateConversationForUserAsync(Guid userId)
        {
            var conversation = await this.chatRepository.GetOrCreateConversationForUserAsync(userId);

            if (conversation == null)
            {
                return null;
            }

            return MapToConversationDto(conversation);
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesForConversationAsync(int conversationId)
        {
            var messages = await this.chatRepository.GetMessagesForConversationAsync(conversationId);

            if (messages == null)
            {
                return Enumerable.Empty<MessageDto>();
            }

            return MapToMessageDtos(messages);
        }

        public async Task<IEnumerable<ConversationDto>> GetConversationsWithMessagesAsync()
        {
            var conversations = await this.chatRepository.GetConversationsWithMessagesAsync();

            if (conversations == null)
            {
                return Enumerable.Empty<ConversationDto>();
            }

            return MapToConversationDtos(conversations);
        }

        public async Task<IEnumerable<ConversationDto>> GetConversationsWhereNutritionistRespondedAsync(Guid nutritionistId)
        {
            var conversations = await this.chatRepository.GetConversationsWhereNutritionistRespondedAsync(nutritionistId);

            if (conversations == null)
            {
                return Enumerable.Empty<ConversationDto>();
            }

            return MapToConversationDtos(conversations);
        }

        public async Task AddMessageAsync(int conversationId, Guid senderId, string text, bool isNutritionist)
        {
            await this.chatRepository.AddMessageAsync(conversationId, senderId, text, isNutritionist);
        }

        public async Task<IEnumerable<ConversationDto>> GetConversationsWithUserMessagesAsync()
        {
            var conversations = await this.chatRepository.GetConversationsWithUserMessagesAsync();

            if (conversations == null)
            {
                return Enumerable.Empty<ConversationDto>();
            }

            return MapToConversationDtos(conversations);
        }

        private static ConversationDto MapToConversationDto(Conversation conversation)
        {
            return new ConversationDto
            {
                ConversationId = conversation.Id,
                HasUnanswered = conversation.HasUnanswered,
                UserId = conversation.User.UserId,
                UserName = conversation.User.Username
            };
        }

        private static List<ConversationDto> MapToConversationDtos(IEnumerable<Conversation> conversations)
        {
            return conversations.Select(MapToConversationDto).ToList();
        }

        private static MessageDto MapToMessageDto(Message message)
        {
            return new MessageDto
            {
                MessageId = message.Id,
                SentAt = message.SentAt,
                ConversationId = message.Conversation.Id,
                SenderUsername = message.Sender.Username,
                SenderRole = message.Sender.Role,
                TextContent = message.TextContent
            };
        }

        private static List<MessageDto> MapToMessageDtos(IEnumerable<Message> messages)
        {
            return messages.Select(MapToMessageDto).ToList();
        }
    }
}
