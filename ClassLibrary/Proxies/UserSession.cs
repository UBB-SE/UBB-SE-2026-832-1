using ClassLibrary.Proxies.Interfaces;

namespace ClassLibrary.Proxies;

public sealed class UserSession : IUserSession
{
    public const string CLIENT_ROLE = "Client";

    private static int currentClientId = 1;
    private static string currentRole = "Nutritionist";

    public int CurrentClientId
    {
        get => currentClientId;
        set => currentClientId = value;
    }

    public string CurrentRole
    {
        get => currentRole;
        set => currentRole = value;
    }

    public string CurrentUserRole
    {
        get => currentRole;
        set => currentRole = value;
    }

    public bool IsClient => currentRole == CLIENT_ROLE;

    public static int? UserId { get; set; }

    public static int? ClientId { get; set; }

    public static string Role
    {
        get => currentRole;
        set => currentRole = value;
    }

    public static void SetCurrentSession(int userId, string role)
    {
        UserId = userId;
        currentRole = role;

        if (role == CLIENT_ROLE)
        {
            ClientId = userId;
            currentClientId = userId;
        }
    }

    public static void Clear()
    {
        UserId = null;
        ClientId = null;
        currentRole = string.Empty;
        currentClientId = 0;
    }
}
