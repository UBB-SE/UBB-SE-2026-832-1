namespace ClassLibrary.DTOs
{
    public class AddMessageRequestDto
    {
        public Guid SenderId { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsNutritionist { get; set; }
    }
}