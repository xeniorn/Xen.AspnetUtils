using Util.AuthorizationHelper.Claims;

namespace Util.AuthorizationHelper.Authentication;

/// <summary>
/// One way of deriving a claim value from a presented api key - a claim type and the transform that produces its
/// value. The transform being null means the key is used as-is.
/// </summary>
/// <remarks>
/// A standard may declare several. That is not an academic case: a key store built on one digest cannot be moved
/// to another, because only the hash was ever kept and the key itself lives with whoever holds it. Emitting every
/// digest lets old and new stores coexist under one scheme instead of forcing a reissue of every key.
/// </remarks>
/// <param name="ClaimType">The claim this digest is carried in.</param>
/// <param name="Transformer">How the key becomes the claim value. Null means verbatim.</param>
public sealed record ApiKeyDigest(string ClaimType, Func<string, string>? Transformer)
{
    /// <summary>The claim a given key produces under this digest.</summary>
    public ClaimDefinition ToClaim(string apiKey)
        => new(ClaimType, Transformer?.Invoke(apiKey) ?? apiKey);
}
