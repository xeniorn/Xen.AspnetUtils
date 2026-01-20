using System.Collections.Frozen;

namespace Util.AuthorizationHelper.Claims;

/// <summary>
/// Signature for a type that contains claim-to-claim mapping, i.e. direct associations of origin claims to mapped claims
/// </summary>
public interface IClaimMappingContainer : IClaimMappingProvider
{
    /// <summary>
    /// All associations present in this mapping container
    /// </summary>
    IReadOnlyCollection<ClaimAssociation> ClaimMapping { get; }

    /// <summary>
    /// Default impl
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<ClaimAssociation>> IClaimMappingProvider.GetMapping(CancellationToken token) => Task.FromResult(ClaimMapping);

    FrozenDictionary<ClaimDefinition, IReadOnlySet<ClaimDefinition>> GetFrozenMapping() 
        => ClaimMapping
        .GroupBy(x => x.SourceClaim)
        .ToFrozenDictionary
        (
            x => x.Key,
            x => (IReadOnlySet<ClaimDefinition>)x.Select(a => a.MappedClaim).ToHashSet()
        );
}