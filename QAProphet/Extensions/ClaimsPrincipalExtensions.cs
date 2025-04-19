using System.Security.Claims;

namespace QAProphet.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
        => principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    
    public static string? GetUserName(this ClaimsPrincipal principal)
    => principal.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
}