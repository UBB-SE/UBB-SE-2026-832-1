namespace WinUI.Services;

public sealed class UserSession : IUserSession
{
    public const string CLIENT_ROLE = "User";

    public int CurrentClientId { get; set; } = 1;

    public string CurrentUserRole { get; set; } = CLIENT_ROLE;

    public bool IsClient => string.Equals(this.CurrentUserRole, CLIENT_ROLE, System.StringComparison.OrdinalIgnoreCase);

    public int? UserId { get; set; }

    public string Role { get; set; }
}
