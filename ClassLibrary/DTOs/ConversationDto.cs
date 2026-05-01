namespace ClassLibrary.DTOs
{
    public class ConversationDto
    {
        public int ConversationId { get; set; }
        public bool HasUnanswered { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
