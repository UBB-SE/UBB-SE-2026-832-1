using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models;

public class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is mandatory")]
    [StringLength(30, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username must be alphanumeric")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is mandatory")]
    [StringLength(30, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is mandatory")]
    [RegularExpression(@"^(User|Nutritionist)$", ErrorMessage = "Role must be 'User' or 'Nutritionist'")]
    public string Role { get; set; } = "User";

    public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
}
