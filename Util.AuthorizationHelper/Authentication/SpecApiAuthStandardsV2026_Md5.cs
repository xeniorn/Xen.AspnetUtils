using Util.AuthorizationHelper.Claims;
using Util.AuthorizationHelper.Util;

namespace Util.AuthorizationHelper.Authentication;

/// <summary>
/// Default implementation of the <see cref="ISpecApiAuthStandards"/>
/// </summary>
public abstract class SpecApiAuthStandardsV2026_Md5 : SpecApiAuthStandardsV2026Base, ISpecApiAuthStandards
{
    /// <inheritdoc />
    public static Func<string, string>? ApiKeyToClaimValueTransformer => x => HashHelper.GetMd5HashHexString(x, lowercase:true);


    /// <inheritdoc />
    public static string DefaultClaimType => MyDefaultClaimType;
    public const string MyDefaultClaimType = "HasApiKeyMd5Hash";

    static ClaimDefinition StandardizedApiKeyClaim(string apiKey, string? customClaimType = null)
        => ISpecApiAuthStandards.StandardizedApiKeyClaim<SpecApiAuthStandardsV2026_Md5>(apiKey, customClaimType);
}