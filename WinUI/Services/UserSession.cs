namespace WinUI.Services;

public sealed class UserSession : IUserSession
{
    public int CurrentClientId { get; set; } = 1;

    public static int? UserId { get; set; }

    public static string Role { get; set; } = string.Empty;

    public static void Clear()
    {
        UserId = null;
        Role = string.Empty;
    }
}