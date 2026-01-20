namespace Util.AuthorizationHelper.Claims;

/// <summary>
/// Provider that can return a claim mapping
/// </summary>
public interface IClaimMappingProvider
{
    Task<IReadOnlyCollection<ClaimAssociation>> GetMapping(CancellationToken token = default);

    /// <summary>
    /// A one-way association from source claim to a mapped (target) claim
    /// </summary>
    /// <param name="SourceClaim"></param>
    /// <param name="MappedClaim"></param>
    public record ClaimAssociation(ClaimDefinition SourceClaim, ClaimDefinition MappedClaim)
    {
        public ClaimAssociation(string sourceClaimType, string sourceClaimValue, string mappedClaimType,
            string mappedClaimValue) : this(new ClaimDefinition(sourceClaimType, sourceClaimValue),
            new ClaimDefinition(mappedClaimType, mappedClaimValue))
        {

        }
    };
}