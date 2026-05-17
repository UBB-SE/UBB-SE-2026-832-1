using ClassLibrary.Proxies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebUI.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class VerifiedClientAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userSession = context.HttpContext.RequestServices.GetRequiredService<IUserSession>();
        if (!userSession.IsClient || userSession.CurrentClientId <= 0)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}
