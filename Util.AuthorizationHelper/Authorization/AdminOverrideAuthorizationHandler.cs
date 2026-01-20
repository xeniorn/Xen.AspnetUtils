using Microsoft.AspNetCore.Authorization;
using Util.AuthorizationHelper.Principals;

namespace Util.AuthorizationHelper.Authorization;

public abstract class AdminOverrideAuthorizationHandler<T> : IAuthorizationHandler where T : IInternalAuthStandard
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (T.SpecialAdminLikeClaim is { } claim && context.User?.HasClaim(claim) == true)
        {
            // Succeed everything that the current policy is asking for
            foreach (var requirement in context.PendingRequirements.ToList())
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
