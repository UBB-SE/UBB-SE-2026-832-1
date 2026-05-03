namespace WinUI.Services;

public sealed class UserSession : IUserSession
{
    public int CurrentClientId { get; set; } = 1;
}
