using Microsoft.Extensions.Logging;
using System.Collections.Frozen;
using Util.AuthorizationHelper.Claims.Extraction.Base.Jwt;

namespace Util.AuthorizationHelper.Claims.Extraction.Presets;

/// <summary>
/// Knows how to extract claims from Azure-issued JWT tokens as per 2026 spec
/// </summary>
/// <typeparam name="TClaimSet"></typeparam>
/// <param name="options"></param>
/// <param name="logger"></param>
public abstract class AzureJwtClaimExtractor2026<TClaimSet>(AzureJwtClaimExtractor2026<TClaimSet>.IMyOptions options, ILogger logger) 
    : JwtClaimExtractor<TClaimSet>(options, logger)
    where TClaimSet: IInternalAuthStandard
{
    public new interface IMyOptions : JwtClaimExtractor<TClaimSet>.IMyOptions
    {
        private const string IssuerClaimType = "iss";
        private const string TenantClaimType = "tid";
        private const string ObjectIdClaimType = "oid";
        //private const string AuthenticationType = "AuthenticationTypes.Federation";
        
        public string? RequiredIssuer { get; } //...login.microsoftonline.com...
        public string? RequiredTenantId { get; }

        // this needs to be set by the app, can't be hardcoded
        ///// <inheritdoc />
        //string JwtClaimExtractor<TClaimSet>.IMyOptions.RequiredAuthenticationType => AuthenticationType;

        /// <inheritdoc />
        ClaimDefinition[] JwtClaimExtractor<TClaimSet>.IMyOptions.RequiredClaims => new List<ClaimDefinition?> () {
            RequiredIssuer is {} issuer ? new ClaimDefinition(IssuerClaimType, issuer) : null,
            RequiredTenantId is {} tenant ? new ClaimDefinition(TenantClaimType, tenant) : null
        }.Where(x => x != null).ToArray()!;

        /// <inheritdoc />
        string[] JwtClaimExtractor<TClaimSet>.IMyOptions.RequiredClaimTypes =>
        [
            IssuerClaimType, TenantClaimType, ObjectIdClaimType
        ];

        /// <inheritdoc />
        MultiMatchBehavior JwtClaimExtractor<TClaimSet>.IMyOptions.MultiMatchBehavior => JwtClaimExtractor<TClaimSet>.MultiMatchBehavior.Error;
    }
}