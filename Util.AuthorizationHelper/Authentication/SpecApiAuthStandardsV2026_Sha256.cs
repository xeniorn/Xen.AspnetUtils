using Util.AuthorizationHelper.Claims;
using Util.AuthorizationHelper.Util;

namespace Util.AuthorizationHelper.Authentication;

/// <summary>
/// Like <see cref="SpecApiAuthStandardsV2026_Md5"/>, but identifies keys by their sha256 hash.
/// <para>
/// Prefer this one for anything new. It reads the same <c>X-Api-Key</c> header, so it can be registered alongside
/// the md5 standard and the two coexist: each handler adds its own identity carrying its own claim type, and a
/// key is recognised by whichever standard its store was built against.
/// </para>
/// <para>
/// Note the overridden <see cref="DefaultSchemeName"/>. Authentication scheme names must be unique, and
/// <see cref="SpecApiAuthStandardsV2026Base"/> gives every standard the same one, so a standard meant to be
/// registered next to another has to name itself.
/// </para>
/// </summary>
public abstract class SpecApiAuthStandardsV2026_Sha256 : SpecApiAuthStandardsV2026Base, ISpecApiAuthStandards
{
    /// <inheritdoc />
    public static Func<string, string>? ApiKeyToClaimValueTransformer => x => HashHelper.GetSha256HashHexString(x, lowercase: true);

    /// <inheritdoc />
    public static string DefaultClaimType => MyDefaultClaimType;

    /// <summary>Distinct from the md5 standard's, so both can be present on the same principal.</summary>
    public const string MyDefaultClaimType = "HasApiKeySha256Hash";

    /// <summary>
    /// Distinct from the shared base scheme name, so this standard can be registered alongside another one.
    /// </summary>
    public new static string DefaultSchemeName => MySchemeName;

    /// <summary>Authentication scheme names must be unique across a host, hence not the inherited one.</summary>
    public const string MySchemeName = "ApiKeySha256";

    static ClaimDefinition StandardizedApiKeyClaim(string apiKey, string? customClaimType = null)
        => ISpecApiAuthStandards.StandardizedApiKeyClaim<SpecApiAuthStandardsV2026_Sha256>(apiKey, customClaimType);
}
