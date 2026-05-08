namespace ClassLibrary.DTOs
{
    public class AddMessageRequestDto
    {
        public int SenderId { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsNutritionist { get; set; }
    }
}