using System.Security.Claims;

namespace SoftPlus.Todo.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var nameIdentifier = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(nameIdentifier) || !int.TryParse(nameIdentifier, out var userId))
        {
            throw new UnauthorizedAccessException("Користувача не ідентифіковано з JWT токена.");
        }

        return userId;
    }
}