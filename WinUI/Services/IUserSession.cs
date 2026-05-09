namespace WinUI.Services;

public interface IUserSession
{
    int CurrentClientId { get; }
    int? UserId { get; set; }
    string Role { get; set; }
}