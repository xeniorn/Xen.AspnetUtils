using Util.AuthorizationHelper.Claims;
using Util.AuthorizationHelper.Util;

namespace Util.AuthorizationHelper.Authentication;

/// <summary>
/// The v2026 api key standard: a key in the <c>X-Api-Key</c> header, recognised by its digest.
/// <para>
/// This is the one to register. It emits every digest the standard knows about, so a key store keyed on md5 and
/// one keyed on sha256 are both matchable from a single authentication scheme.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// Sha256 is the primary digest - it is what <see cref="DefaultClaimType"/> and
/// <see cref="ApiKeyToClaimValueTransformer"/> describe, so anything deriving a hash from this standard produces
/// sha256, and new key stores use it.
/// </para>
/// <para>
/// Md5 is still emitted because it cannot be abandoned unilaterally: an existing store holds md5 hashes and
/// nothing else, and a hash cannot be recomputed into another digest without the key, which lives with its holder.
/// Dropping the md5 claim would silently stop honouring every key issued so far. It goes when those keys are
/// reissued, not before.
/// </para>
/// <para>
/// Since 256 bits of key entropy puts guessing far out of reach, the digest only has to be a fast one-way
/// function. A password KDF would be the wrong tool here.
/// </para>
/// </remarks>
public abstract class SpecApiAuthStandardsV2026 : SpecApiAuthStandardsV2026Base, ISpecApiAuthStandards
{
    /// <summary>Sha256 hex, lowercase.</summary>
    public static Func<string, string>? ApiKeyToClaimValueTransformer => Sha256.Transformer;

    /// <inheritdoc />
    public static string DefaultClaimType => Sha256.ClaimType;

    /// <summary>The digest new key stores should use.</summary>
    public static readonly ApiKeyDigest Sha256 = new(
        Sha256ClaimType,
        x => HashHelper.GetSha256HashHexString(x, lowercase: true));

    /// <summary>Kept for stores that predate <see cref="Sha256"/>; see the remarks on the class.</summary>
    public static readonly ApiKeyDigest Md5 = new(
        SpecApiAuthStandardsV2026_Md5.MyDefaultClaimType,
        x => HashHelper.GetMd5HashHexString(x, lowercase: true));

    /// <inheritdoc />
    public static IReadOnlyCollection<ApiKeyDigest> Digests => [Sha256, Md5];

    /// <summary>Claim type carrying the sha256 digest.</summary>
    public const string Sha256ClaimType = "HasApiKeySha256Hash";

    static ClaimDefinition StandardizedApiKeyClaim(string apiKey, string? customClaimType = null)
        => ISpecApiAuthStandards.StandardizedApiKeyClaim<SpecApiAuthStandardsV2026>(apiKey, customClaimType);
}
