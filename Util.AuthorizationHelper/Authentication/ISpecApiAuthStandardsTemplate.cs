using Util.AuthorizationHelper.Claims;

namespace Util.AuthorizationHelper.Authentication;

/// <summary>
/// A template for standard names used in special api auth
/// </summary>
public interface ISpecApiAuthStandards
{
    /// <summary>
    /// 
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
}