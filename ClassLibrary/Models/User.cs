using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class User
{
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;
}
