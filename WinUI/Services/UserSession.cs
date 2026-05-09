namespace WinUI.Services;

public sealed class UserSession : IUserSession
{
    public int CurrentClientId { get; set; } = 1;

    public int? UserId { get; set; } = 5;

    public string Role { get; set; } = "Nutritionist";
}
