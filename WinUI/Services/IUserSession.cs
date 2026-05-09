namespace WinUI.Services;

public interface IUserSession
{
    int CurrentClientId { get; }

    string CurrentRole { get; }

    string CurrentUserRole { get; }

    bool IsClient { get; }
}
