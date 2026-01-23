using System.Security.Claims;

namespace Util.AuthorizationHelper.Claims.Extraction.Base;

/// <summary>
/// Provides a base class for extracting internal claim definitions from single or multiple relevant identities within a claims principal.
/// </summary>
/// <typeparam name="TClaimSet">The type of internal claim set to extract, which must implement <see cref="IInternalAuthStandard"/>.</typeparam>
public abstract class MultiInternalClaimExtractorBase<TClaimSet> : IInternalClaimExtractor<TClaimSet>
    where TClaimSet : IInternalAuthStandard
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ClaimDefinition>> GetClaimDefinitions(ClaimsPrincipal principal, CancellationToken token)
    {
        if (await GetRelevantIdentities(principal, token) is not { } identities || identities.Count == 0)
        {
            return [];
        }

        var claims = await GetClaims(identities, token);
        return claims;
    }

    /// <summary>
    /// Obtain claims from the provided identities
    /// </summary>
    /// <param name="identities"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract Task<IReadOnlyCollection<ClaimDefinition>> GetClaims(IReadOnlyCollection<ClaimsIdentity> identities, CancellationToken token = default);

    /// <summary>
    /// Obtain the relevant identities
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract Task<IReadOnlyCollection<ClaimsIdentity>> GetRelevantIdentities(ClaimsPrincipal principal, CancellationToken token = default);

}