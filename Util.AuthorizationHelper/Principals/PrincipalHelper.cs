using System.Security.Claims;
using Util.AuthorizationHelper.Claims;

namespace Util.AuthorizationHelper.Principals;

/// <summary>
/// Extension methods for less verbose setup of internal policies and claims
/// </summary>
public static class PrincipalHelper
{
    /// <summary>
    /// Helper to check if a ClaimsPrincipal has a specific claim using the strongly typed claim definition
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <param name="requiredClaim"></param>
    /// <returns></returns>
    public static bool HasClaim(this ClaimsPrincipal claimsPrincipal, ClaimDefinition requiredClaim)
    {
        foreach (var identity in claimsPrincipal.Identities)
        {
            var hasClaim = identity.HasClaim(c => c.Type == requiredClaim.ClaimType && c.Value == requiredClaim.Value);
            if (hasClaim)
                return true;
        }

        return false;

        //return claimsPrincipal.HasClaim(c => c.Type == requiredClaim.ClaimType && c.Value == requiredClaim.Value);
    }

    /// <summary>
    /// Determines whether the specified principal possesses all of the required claims.
    /// </summary>
    /// <param name="claimsPrincipal">The principal whose claims are to be evaluated. Cannot be null.</param>
    /// <param name="requiredClaims">A collection of claim definitions that must be present in the principal. Cannot be null or contain null
    /// elements.</param>
    /// <returns>true if the principal contains all the required claims; otherwise, false.</returns>
    public static bool HasClaims(this ClaimsPrincipal claimsPrincipal, IReadOnlyCollection<ClaimDefinition> requiredClaims)
    {
        var hasAllClaims = requiredClaims.All(claimsPrincipal.HasClaim);
        return hasAllClaims;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="AuthType"></param>
    /// <param name="Claims"></param>
    public record IdentityDefinition(string AuthType, IReadOnlyCollection<ClaimDefinition> Claims)
    {
        // TODO: move to ext method, keep the record clean / free from deps
        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static IdentityDefinition FromIdentity(ClaimsIdentity identity)
            => new IdentityDefinition(identity?.AuthenticationType ?? string.Empty, identity?.Claims.Select(ClaimDefinition.FromClaim).ToArray() ?? []);


    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Identities"></param>
    public record ClaimsPrincipalDefinition(IReadOnlyCollection<IdentityDefinition> Identities)
    {
        // TODO: move to ext method, keep the record clean / free from deps
        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static ClaimsPrincipalDefinition FromPrincipal(ClaimsPrincipal principal)
            => new ClaimsPrincipalDefinition(principal.Identities.Select(IdentityDefinition.FromIdentity).ToArray());

    }
}