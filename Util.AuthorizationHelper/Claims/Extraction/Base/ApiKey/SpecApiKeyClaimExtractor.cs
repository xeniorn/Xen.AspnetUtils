using System.Security.Claims;
using Util.AuthorizationHelper.Authentication;

namespace Util.AuthorizationHelper.Claims.Extraction.Base.ApiKey;

/// <summary>
/// 
/// </summary>
/// <param name="options"></param>
/// <typeparam name="TClaimSet"></typeparam>
/// <typeparam name="TApiAuthStandard"></typeparam>
public abstract class SpecApiKeyClaimExtractor<TClaimSet, TApiAuthStandard>(SpecApiKeyClaimExtractor<TClaimSet, TApiAuthStandard>.IMyOptions options) 
    : FilteredIdentityClaimExtractor<TClaimSet>(options) 
    where TClaimSet : IInternalAuthStandard
    where TApiAuthStandard : ISpecApiAuthStandards
{
    /// <summary>
    /// 
    /// </summary>
    public new interface IMyOptions : FilteredIdentityClaimExtractor<TClaimSet>.IMyOptions
    {
        protected static readonly string RequiredSchemeName = TApiAuthStandard.DefaultSchemeName;
        Func<ClaimsIdentity, bool> FilteredIdentityClaimExtractor<TClaimSet>.IMyOptions.IdentityFilter 
            => (identity => identity.IsAuthenticated && identity.AuthenticationType == RequiredSchemeName);
    }
}