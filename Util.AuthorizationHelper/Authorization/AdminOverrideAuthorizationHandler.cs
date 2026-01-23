using Microsoft.AspNetCore.Authorization;
using Util.AuthorizationHelper.Principals;

namespace Util.AuthorizationHelper.Authorization;

/// <summary>
/// Can be used directly, or inherited as an app-specific handler.
/// Makes every requirement automatically pass completely disregarding what it is
/// </summary>
/// <typeparam name="TStandard"></typeparam>
public class AdminOverrideAuthorizationHandler<TStandard> : IAuthorizationHandler where TStandard : IInternalAuthStandard
{
    /// <inheritdoc />
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (TStandard.SpecialAdminLikeClaim is { } claim && context.User?.HasClaim(claim) == true)
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
