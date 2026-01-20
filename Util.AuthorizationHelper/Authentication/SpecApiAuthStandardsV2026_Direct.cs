using Util.AuthorizationHelper.Claims;

namespace Util.AuthorizationHelper.Authentication;

/// <summary>
/// Default implementation of the <see cref="ISpecApiAuthStandards"/>
/// </summary>
public abstract class SpecApiAuthStandardsV2026_Direct : SpecApiAuthStandardsV2026Base, ISpecApiAuthStandards
{
    /// <inheritdoc />
    public static Func<string, string>? ApiKeyToClaimValueTransformer => null;
    
    /// <inheritdoc />
    public static string DefaultClaimType =>MyDefaultClaimType;
    public const string MyDefaultClaimType = "HasApiKey";

    static ClaimDefinition StandardizedApiKeyClaim(string apiKey, string? customClaimType = null)
        => ISpecApiAuthStandards.StandardizedApiKeyClaim<SpecApiAuthStandardsV2026_Direct>(apiKey, customClaimType);
}