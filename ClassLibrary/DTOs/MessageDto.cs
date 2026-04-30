namespace ClassLibrary.DTOs
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public DateTime SentAt { get; set; }
        public int ConversationId { get; set; }
        public string SenderUsername { get; set; } = string.Empty;
        public string SenderRole { get; set; } = string.Empty;
        public string TextContent { get; set; } = string.Empty;
        public bool IsFromCurrentUser { get; set; }
    }
}