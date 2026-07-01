namespace Web.Extensions;

public static class HttpContextExtensions
{
    private const string AnonymousBuyerCookieName = "eShop_BuyerId";

    // Identifies the basket owner: the authenticated username when signed in, otherwise a
    // stable per-browser id stored in a cookie so an anonymous basket survives across requests.
    public static string GetOrCreateBuyerId(this HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            return context.User.Identity.Name!;
        }

        if (context.Request.Cookies.TryGetValue(AnonymousBuyerCookieName, out var existingId) &&
            !string.IsNullOrEmpty(existingId))
        {
            return existingId;
        }

        var newId = Guid.NewGuid().ToString();
        context.Response.Cookies.Append(AnonymousBuyerCookieName, newId, new CookieOptions
        {
            IsEssential = true,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
        });
        return newId;
    }

    public static string? GetAnonymousBuyerIdCookie(this HttpContext context)
    {
        return context.Request.Cookies.TryGetValue(AnonymousBuyerCookieName, out var id) ? id : null;
    }

    public static void ClearAnonymousBuyerIdCookie(this HttpContext context)
    {
        context.Response.Cookies.Delete(AnonymousBuyerCookieName);
    }
}
