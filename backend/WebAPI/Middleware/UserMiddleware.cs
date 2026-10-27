using Common.Interfaces;
using Common.Models;

namespace WebAPI.Middleware;

public class UserMiddleware(RequestDelegate next, ICrudFactory crudFactory)
{
    public const string HeaderName = "X-User-Id";
    public const string UserIdItemKey = "HomeHubUserId";

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out var values))
        {
            var userId = values.ToString();
            var user = await crudFactory.GetCrud<AppUser>().GetByIdAsync(userId);
            if (user == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            context.Items[UserIdItemKey] = userId;
        }

        await next(context);
    }
}
