using System.Security.Claims;

namespace Util.AuthorizationHelper.Claims.Extraction.Base;

/// <summary>
/// Provides a base class for extracting internal claim definitions from single or multiple relevant identities within a claims principal, where the identities are selected using a custom per-identity filter.
/// </summary>
/// <typeparam name="TClaimSet">The type of internal claim set to extract, which must implement <see cref="IInternalAuthStandard"/>.</typeparam>
public abstract class FilteredIdentityClaimExtractor<TClaimSet>(FilteredIdentityClaimExtractor<TClaimSet>.IMyOptions options) : MultiInternalClaimExtractorBase<TClaimSet>
    where TClaimSet : IInternalAuthStandard
{
    public interface IMyOptions
    {
        public Func<ClaimsIdentity, bool> IdentityFilter { get; }
    }

    /// <inheritdoc />
    protected override Task<IReadOnlyCollection<ClaimsIdentity>> GetRelevantIdentities(ClaimsPrincipal principal, CancellationToken token = default)
    {
        var res = principal.Identities.Where(options.IdentityFilter).ToArray();
        return Task.FromResult<IReadOnlyCollection<ClaimsIdentity>>(res);
    }

}