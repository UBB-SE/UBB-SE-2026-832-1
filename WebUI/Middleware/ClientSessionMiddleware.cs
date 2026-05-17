using ClassLibrary.Proxies;
using WebUI.Services;

namespace WebUI.Middleware;

public sealed class ClientSessionMiddleware
{
    private readonly RequestDelegate next;

    public ClientSessionMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        if (context.Session.GetInt32("ClientId") == null)
        {
            var clientId = configuration.GetValue<int?>("ClientSession:ClientId");
            var role = configuration.GetValue<string>("ClientSession:Role");
            if (clientId is > 0 && role == UserSession.CLIENT_ROLE)
            {
                HttpContextUserSession.SetSession(context.Session, clientId.Value, role);
            }
        }

        await this.next(context).ConfigureAwait(false);
    }
}
