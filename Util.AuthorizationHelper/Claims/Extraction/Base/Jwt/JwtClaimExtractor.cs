using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Util.AuthorizationHelper.Claims.Extraction.Base.Jwt;

/// <summary>
/// Knows how to extract claims from JWT tokens.
/// Uses only a single relevant identity within the claims principal, defined by the constraints in <see cref="IMyOptions"/>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="options"></param>
/// <param name="logger"></param>
public abstract class JwtClaimExtractor<T>(JwtClaimExtractor<T>.IMyOptions options, ILogger logger)
    : SimpleInternalClaimExtractor<T>
    where T : IInternalAuthStandard
{
    /// <summary>
    /// Should primarily rely on <see cref="RequiredAuthenticationType"/>
    /// </summary>
    public interface IMyOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string RequiredAuthenticationType { get; } //= JwtBearerDefaults.AuthenticationScheme;

        /// <summary>
        /// Avoid using this unless really required
        /// </summary>
        public ClaimDefinition[] RequiredClaims { get; }

        /// <summary>
        /// Avoid using this unless really required
        /// </summary>
        public string[] RequiredClaimTypes { get; }

        /// <summary>
        /// For convenience, but in most cases should be set to <see cref="MultiMatchBehavior.Error"/> as there shouldn't be multiples of the same auth Type
        /// </summary>
        public MultiMatchBehavior MultiMatchBehavior { get; }


    }

    public enum MultiMatchBehavior
    {
        Error,
        FirstMatch,
        LastMatch,
    }

    public static class MyExceptions
    {
        /// <summary>
        /// Multiple identities matched the requested constraints and the <see cref="MultiMatchBehavior "/> is set to <see cref="MultiMatchBehavior.Error"/>
        /// </summary>
        public class MultipleMatchingIdentitiesException : InvalidOperationException
        {
            public MultipleMatchingIdentitiesException(string message) : base(message)
            {
            }
        }
    }

#if DEBUG
    private record NamedCondition(string Name, Predicate<ClaimsIdentity> Predicate);
#endif

    protected sealed override Task<ClaimsIdentity?> GetRelevantIdentity(ClaimsPrincipal principal, CancellationToken token = default)
    {
        #if DEBUG
            IReadOnlyCollection<ClaimsIdentity> putativeIds = principal.Identities.ToArray();

            putativeIds = putativeIds.Where(x => x.IsAuthenticated).ToArray();

            putativeIds = putativeIds.Where(x => x.AuthenticationType == options.RequiredAuthenticationType).ToArray();

            foreach (var type in options.RequiredClaimTypes)
            {
                putativeIds = putativeIds.Where(x => x.HasClaim(c => c.Type == type)).ToArray();
            }

            foreach (var reqClaim in options.RequiredClaims)
            {
                putativeIds = putativeIds.Where(x => x.HasClaim(c => c.Type == reqClaim.ClaimType && c.Value == reqClaim.Value)).ToArray();
            }

            NamedCondition[] filters =
            [
                new("isAuth", x => x.IsAuthenticated),
                new("corrAuthType", x => x.AuthenticationType == options.RequiredAuthenticationType),

                ..options.RequiredClaimTypes
                    .Select(type => new NamedCondition(
                        $"hasClaimType_{type}",
                        x => x.HasClaim(c => c.Type == type))
                    ),

                ..options.RequiredClaims
                    .Select(reqClaim => new NamedCondition(
                        $"hasClaim_{reqClaim.ClaimType}_{reqClaim.Value}",
                        x => x.HasClaim(c => c.Type == reqClaim.ClaimType && c.Value == reqClaim.Value))
                    )
            ];

            var res = filters.ToDictionary
            (
                x => x.Name,
                x => principal.Identities.Where(i => x.Predicate(i)).ToArray()
            );

#endif

        //putativeIds = putativeIds.Where(x => x.IsAuthenticated).ToArray();

        // NOTE: in practice we should only match on authentication type. Each authentication method is required to have its own authentication type (framework prevents multiple),
        // so matching on that should provide a unique result.
        // Malicious plugin authentication mimincking the real one could go around it, but it would require the ability to plug in / alter code which would be a problem in itself if malicious
        // a full security model would require checks at multiple levels and signature validation and blabla, which is well beyond the scope of this library
        var matchingIdentites = principal.Identities
            .Where(x => x.IsAuthenticated)
            .Where(x => x.AuthenticationType == options.RequiredAuthenticationType)
            .Where(x => options.RequiredClaimTypes
                .All(reqType => x.HasClaim(c => c.Type == reqType)))
            .Where(x => options.RequiredClaims
                .All(reqClaim => x.HasClaim(c => c.Type == reqClaim.ClaimType && c.Value == reqClaim.Value)))
            .ToArray();
        
        return Task.FromResult(matchingIdentites.Length switch
        {
            0 => null,
            1 => matchingIdentites.Single(),
            _ => options.MultiMatchBehavior switch
            {
                MultiMatchBehavior.Error => throw new MyExceptions.MultipleMatchingIdentitiesException($"Multiple matching identities found for authentication type {options.RequiredAuthenticationType} with the requested claim constraints"),
                MultiMatchBehavior.FirstMatch => matchingIdentites.First(),
                MultiMatchBehavior.LastMatch => matchingIdentites.Last(),
                _ => throw new InvalidOperationException("Unknown MultiMatchBehavior")
            }
        });
    }

}
