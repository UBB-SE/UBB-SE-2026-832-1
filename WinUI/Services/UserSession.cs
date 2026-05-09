namespace WinUI.Services;

public sealed class UserSession : IUserSession
{
    public int CurrentClientId { get; set; }

    public int? UserId { get; set; }

    public string Role { get; set; }
}
