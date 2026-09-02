using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Util.AuthorizationHelper.Authentication;
using Util.AuthorizationHelper.DI;

namespace Util.AuthorizationHelper.Test;

/// <summary>
/// The v2026 standard emits several digests from one key, so that key stores built on different digests can be
/// matched from a single authentication scheme. This is the alternative to registering a second scheme per
/// digest, which would collide on the scheme name.
/// </summary>
public class ApiKeyDigestTests
{
    private const string TestApiKey = "blaZaraBla123";
    private const string Md5OfTestKey = "4111d3bd7d3bd2842ad0e84b77290e0f";
    private const string Sha256OfTestKey = "ab91fe8a7400494d01674d2933d3587227ddbb92c92ca30d0c31f4d97f34be3f";

    private static IReadOnlyList<Claim> ClaimsFromHandler<TStandard>(string apiKey)
        where TStandard : ISpecApiAuthStandards
    {
        var request = new DefaultHttpRequest(new DefaultHttpContext())
        {
            Headers = { [TStandard.DefaultHeaderName] = apiKey }
        };

        var ticket = SpecApiAuthHandler<TStandard>.GetAuthenticationTicketFromRequest(request, NullLogger.Instance);

        return ticket!.Principal.Identities.Single().Claims.ToArray();
    }

    /// <summary>
    /// Both digests, one identity, one scheme. An md5-keyed store and a sha256-keyed store can each find their own
    /// claim without either knowing the other exists.
    /// </summary>
    [Fact]
    public void OneKeyProducesEveryDigestTheStandardDeclares()
    {
        var claims = ClaimsFromHandler<SpecApiAuthStandardsV2026>(TestApiKey);

        Assert.Equal(2, claims.Count);
        Assert.Equal(Sha256OfTestKey, claims.Single(x => x.Type == SpecApiAuthStandardsV2026.Sha256.ClaimType).Value);
        Assert.Equal(Md5OfTestKey, claims.Single(x => x.Type == SpecApiAuthStandardsV2026.Md5.ClaimType).Value);
    }

    /// <summary>
    /// The md5 claim type has to stay exactly what the older standard used, or every key already issued stops
    /// being recognised - and no hash can be recomputed into the new digest, since only the hash was kept.
    /// </summary>
    [Fact]
    public void Md5ClaimIsIdenticalToWhatTheOlderStandardEmitted()
    {
        var combined = ClaimsFromHandler<SpecApiAuthStandardsV2026>(TestApiKey);
        var md5Only = ClaimsFromHandler<SpecApiAuthStandardsV2026_Md5>(TestApiKey);

        var expected = md5Only.Single();
        var actual = combined.Single(x => x.Type == expected.Type);

        Assert.Equal(expected.Value, actual.Value);
    }

    /// <summary>Anything deriving a hash from the standard gets the primary digest, so new stores use sha256.</summary>
    [Fact]
    public void PrimaryDigestIsSha256()
    {
        var claim = ISpecApiAuthStandards.StandardizedApiKeyClaim<SpecApiAuthStandardsV2026>(TestApiKey);

        Assert.Equal(SpecApiAuthStandardsV2026.Sha256.ClaimType, claim.ClaimType);
        Assert.Equal(Sha256OfTestKey, claim.Value);
    }

    /// <summary>One scheme, so nothing collides and nothing has to override the shared scheme name.</summary>
    [Fact]
    public void RegistersAsASingleScheme()
    {
        var services = new ServiceCollection();
        services.AddAuthentication().AddSpecialApiKeyAuthentication<SpecApiAuthStandardsV2026>();

        var schemes = services.BuildServiceProvider()
            .GetRequiredService<IOptions<AuthenticationOptions>>().Value
            .Schemes.Select(x => x.Name).ToArray();

        Assert.Equal([SpecApiAuthStandardsV2026Base.MyDefaultSchemeName], schemes);
    }

    /// <summary>
    /// A standard declaring no digests still emits its single primary one, so the older standards are unaffected.
    /// </summary>
    [Fact]
    public void StandardsDeclaringNoDigestsKeepEmittingExactlyOneClaim()
    {
        Assert.Single(ClaimsFromHandler<SpecApiAuthStandardsV2026_Md5>(TestApiKey));
        Assert.Single(ClaimsFromHandler<SpecApiAuthStandardsV2026_Direct>(TestApiKey));
    }
}
