using Util.AuthorizationHelper.Claims;

namespace Util.AuthorizationHelper.Authentication;

/// <summary>
/// A template for standard names used in special api auth
/// </summary>
public interface ISpecApiAuthStandards
{
    /// <summary>
    /// Creates a standardized <see cref="ClaimDefinition"/> for the provided api key. The definition will conform to the provided
    /// <see cref="ISpecApiAuthStandards"/> standard, including claim type & transformations
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="customClaimType">In case you want to override the standard one (which you shouldn't generally do)</param>
    /// <returns></returns>
    static ClaimDefinition StandardizedApiKeyClaim<T>(string apiKey, string? customClaimType = null) where T: ISpecApiAuthStandards
        => new ClaimDefinition
            (
                customClaimType ??  T.DefaultClaimType,
                T.ApiKeyToClaimValueTransformer?.Invoke(apiKey) ?? apiKey);

    /// <summary>
    /// Transformer from API key to the value that will be in the claim. Normally it would be a trivial transformer x=>x,
    /// but implementations could choose to e.g. compare MD5 hashes instead.
    /// If null, is equivalent to trivial implementation
    /// </summary>
    /// <returns></returns>
    public static abstract Func<string, string>? ApiKeyToClaimValueTransformer { get; }

    public static abstract string DefaultHeaderName { get; }
    public static abstract string DefaultSchemeName { get; }
    public static abstract string DefaultClaimType { get; }

    /// <summary>
    /// Every claim a presented key should produce, when the standard supports more than one digest.
    /// <para>
    /// Empty - the default - means the single digest described by <see cref="DefaultClaimType"/> and
    /// <see cref="ApiKeyToClaimValueTransformer"/>. Declaring several lets key stores built on different digests
    /// coexist under one authentication scheme, which matters because a stored hash can never be recomputed into
    /// another digest: only the hash was kept.
    /// </para>
    /// </summary>
    public static virtual IReadOnlyCollection<ApiKeyDigest> Digests => [];

    /// <summary>
    /// The digests <typeparamref name="T"/> actually emits, resolving the default when it declares none.
    /// </summary>
    static IReadOnlyCollection<ApiKeyDigest> DigestsOf<T>() where T : ISpecApiAuthStandards
        => T.Digests.Count > 0
            ? T.Digests
            : [new ApiKeyDigest(T.DefaultClaimType, T.ApiKeyToClaimValueTransformer)];

    /// <summary>
    /// Every standardized claim the api key produces under <typeparamref name="T"/>.
    /// </summary>
    static IReadOnlyCollection<ClaimDefinition> StandardizedApiKeyClaims<T>(string apiKey)
        where T : ISpecApiAuthStandards
        => DigestsOf<T>().Select(x => x.ToClaim(apiKey)).ToArray();
}