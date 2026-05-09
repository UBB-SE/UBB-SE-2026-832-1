using System;

namespace WinUI.Services;

public sealed class UserSession : IUserSession
{
public const string CLIENT_ROLE = "Client";

    private static int currentClientId = 1;
    private static string currentRole = "Client";

    public int CurrentClientId => currentClientId;

    public string CurrentRole => currentRole;

    public string CurrentUserRole { get; set; } = currentRole;

    public bool IsClient => string.Equals(this.CurrentUserRole, CLIENT_ROLE, System.StringComparison.OrdinalIgnoreCase);

    public static void SetCurrentSession(int clientId, string role)
    {
        currentClientId = clientId;
        currentRole = NormalizeRole(role);
    }

    private static string NormalizeRole(string role)
    {
        if (string.Equals(role, "User", StringComparison.OrdinalIgnoreCase))
        {
            return "Client";
        }

        return role;
    }
}
