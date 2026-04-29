using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models
{
    public class Reminder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool HasSound { get; set; } = false;

        [Required]
        public TimeSpan Time { get; set; }

        public string? ReminderDate { get; set; }

        [MaxLength(50)]
        public string Frequency { get; set; } = "Once";
    }
}