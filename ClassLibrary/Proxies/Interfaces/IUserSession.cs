namespace ClassLibrary.Proxies.Interfaces;

public interface IUserSession
{
    int CurrentClientId { get; }

    string CurrentRole { get; }

    string CurrentUserRole { get; }

    bool IsClient { get; }
}



