using System.Security.Claims;
using Util.AuthorizationHelper.Authentication;

namespace Util.AuthorizationHelper.Claims.Extraction.Base.ApiKey;

/// <summary>
/// Assigns claims based on ApiKey authentication(s), using a predefined mapping from API key to Claims. Uses an arbitrary standard <see cref="TApiAuthStandard"/> which needs to be a <see cref="ISpecApiAuthStandards"/>
/// Typically you'd go for the default standard and use the derived class <see cref="SpecApiKeyMapBasedClaimExtractor{TClaimSet}"/>
/// </summary>
/// <typeparam name="TClaimSet"></typeparam>
/// <typeparam name="TApiAuthStandard"></typeparam>
/// <param name="options"></param>
public abstract class SpecApiKeyMapBasedClaimExtractor<TClaimSet, TApiAuthStandard>(SpecApiKeyMapBasedClaimExtractor<TClaimSet, TApiAuthStandard>.IMyOptions options) 
    : SpecApiKeyClaimExtractor<TClaimSet, TApiAuthStandard>(options) 
    where TClaimSet : IInternalAuthStandard
    where TApiAuthStandard : ISpecApiAuthStandards
{
    public new interface IMyOptions : SpecApiKeyClaimExtractor<TClaimSet, TApiAuthStandard>.IMyOptions, IClaimMappingContainer
    {
        /// <summary>
        /// List of claims to be granted to a matching API key or its associated value (in case the standard uses an API key transformer)
        /// </summary>
        public IReadOnlyDictionary<string, ClaimDefinition[]> ApiKeyAssociatedValueToClaimsMap { get; }

        IReadOnlyCollection<IClaimMappingProvider.ClaimAssociation> IClaimMappingContainer.ClaimMapping
            => ApiKeyAssociatedValueToClaimsMap.SelectMany(kvp => kvp.Value
                .Select(mappedClaim =>
                {
                    var apiKeyClaim = ISpecApiAuthStandards.StandardizedApiKeyClaim<TApiAuthStandard>(kvp.Key);
                    return new IClaimMappingProvider.ClaimAssociation(apiKeyClaim, mappedClaim);
                })
            ).ToArray();
    }

    private IReadOnlyDictionary<ClaimDefinition, IReadOnlySet<ClaimDefinition>> CachedMap { get; } = options.GetFrozenMapping();

    /// <inheritdoc />
    protected override Task<IReadOnlyCollection<ClaimDefinition>> GetClaims(IReadOnlyCollection<ClaimsIdentity> identities, CancellationToken token = default)
    {
        var allSourceClaims = identities
            .SelectMany(x => x.Claims)
            .Select(x=> new ClaimDefinition(x.Type, x.Value));

        var claims = allSourceClaims
            .SelectMany(c => CachedMap.GetValueOrDefault(c, new HashSet<ClaimDefinition>()))
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<ClaimDefinition>>(claims);
    }
}


/// <summary>
/// Default version of <see cref="SpecApiKeyMapBasedClaimExtractor{TClaimSet,TApiAuthStandard}"/> that uses the common <see cref="SpecApiAuthStandardsV2026_Direct"/> standard. 
/// </summary>
/// <typeparam name="TClaimSet"></typeparam>
/// <param name="options"></param>
public abstract class SpecApiKeyMapBasedClaimExtractor<TClaimSet>(SpecApiKeyMapBasedClaimExtractor<TClaimSet, SpecApiAuthStandardsV2026_Direct>.IMyOptions options) : SpecApiKeyMapBasedClaimExtractor<TClaimSet, SpecApiAuthStandardsV2026_Direct>(options)
    where TClaimSet : IInternalAuthStandard
{
}

