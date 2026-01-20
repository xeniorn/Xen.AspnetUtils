using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Util.AuthorizationHelper.Claims.Extraction.Base.Jwt;

namespace Util.AuthorizationHelper.Claims.Extraction.Presets;

public abstract class AzureJwtMapBasedClaimExtractor2026<TClaimSet>(
    AzureJwtMapBasedClaimExtractor2026<TClaimSet>.IMyOptions options,
    ILogger logger) : JwtClaimExtractor<TClaimSet>(options, logger)
    where TClaimSet : IInternalAuthStandard
{
    public new interface IMyOptions : JwtClaimExtractor<TClaimSet>.IMyOptions, IClaimMappingContainer
    {
        
    }

    private IReadOnlyDictionary<ClaimDefinition, IReadOnlySet<ClaimDefinition>> CachedMap { get; } = options.GetFrozenMapping();
        

    /// <inheritdoc />
    protected override Task<IReadOnlyCollection<ClaimDefinition>> GetClaims(ClaimsIdentity identity, CancellationToken token = default)
    {
        var allSourceClaims = identity.Claims
            .Select(x => new ClaimDefinition(x.Type, x.Value));

        var claims = allSourceClaims
            .SelectMany(c => CachedMap.GetValueOrDefault(c, new HashSet<ClaimDefinition>()))
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<ClaimDefinition>>(claims);
    }
}