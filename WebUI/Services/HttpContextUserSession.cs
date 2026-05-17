using ClassLibrary.Proxies;
using ClassLibrary.Proxies.Interfaces;

namespace WebUI.Services;

public sealed class HttpContextUserSession : IUserSession
{
    private const string ClientIdKey = "ClientId";
    private const string RoleKey = "Role";

    private readonly IHttpContextAccessor httpContextAccessor;

    public HttpContextUserSession(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public int CurrentClientId =>
        this.httpContextAccessor.HttpContext?.Session.GetInt32(ClientIdKey) ?? 0;

    public string CurrentRole =>
        this.httpContextAccessor.HttpContext?.Session.GetString(RoleKey) ?? string.Empty;

    public string CurrentUserRole => this.CurrentRole;

    public bool IsClient => this.CurrentRole == UserSession.CLIENT_ROLE;

    public static void SetSession(ISession session, int clientId, string role)
    {
        session.SetInt32(ClientIdKey, clientId);
        session.SetString(RoleKey, role);
    }
}
