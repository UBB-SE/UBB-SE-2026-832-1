namespace WinUI.Services;

public sealed class UserSession : IUserSession
{
    public const string CLIENT_ROLE = "User";

    public int CurrentClientId { get; set; } = 2;

    public static int? UserId { get; set; }

    public static string Role { get; set; } = string.Empty;

    public string CurrentUserRole { get; set; } = CLIENT_ROLE;

    public bool IsClient => string.Equals(this.CurrentUserRole, CLIENT_ROLE, System.StringComparison.OrdinalIgnoreCase);

    public static void Clear()
    {
        UserId = null;
        Role = string.Empty;
    }
}
