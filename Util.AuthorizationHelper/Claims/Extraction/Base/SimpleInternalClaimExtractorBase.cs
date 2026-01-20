using System.Security.Claims;

namespace Util.AuthorizationHelper.Claims.Extraction.Base;

/// <summary>
/// Provides a base class for extracting internal claim definitions from a single relevant identity within a claims principal.
/// </summary>
/// <typeparam name="TClaimSet">The type of internal claim set to extract, which must implement <see cref="IInternalAuthStandard"/>.</typeparam>
public abstract class SimpleInternalClaimExtractor<TClaimSet> : IInternalClaimExtractor<TClaimSet>
    where TClaimSet : IInternalAuthStandard
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ClaimDefinition>> GetClaimDefinitions(ClaimsPrincipal principal, CancellationToken token = default)
    {
        if (await GetRelevantIdentity(principal, token) is not { } identity)
        {
            return [];
        }

        var claims = await GetClaims(identity, token);
        return claims;
    }

    /// <summary>
    /// Obtain claims from the relevant identity
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract Task<IReadOnlyCollection<ClaimDefinition>> GetClaims(ClaimsIdentity identity, CancellationToken token = default);

    /// <summary>
    /// Obtain the relevant identity, if it exists
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract Task<ClaimsIdentity?> GetRelevantIdentity(ClaimsPrincipal principal, CancellationToken token = default);

}