using System.Security.Claims;

namespace Util.AuthorizationHelper.Claims.Extraction.Base;

/// <summary>
/// Extracts internal claims from the ClaimsPrincipal according to some logic.
/// E.g. could have 1 such for Azure AD, 1 for IdentityServer, 1 for API auth etc
/// </summary>
public interface IInternalClaimExtractor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<ClaimDefinition>> GetClaimDefinitions(ClaimsPrincipal principal, CancellationToken token = default);
}

/// <summary>
/// Extracts internal claims from the ClaimsPrincipal according to some logic.
/// E.g. could have 1 such for Azure AD, 1 for IdentityServer, 1 for API auth etc.
/// Restricted to a specific internal claim set type T.
/// </summary>
public interface IInternalClaimExtractor<T> : IInternalClaimExtractor where T : IInternalAuthStandard
{
}