using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Util.AuthorizationHelper.Authentication;
using Util.AuthorizationHelper.Claims;
using Util.AuthorizationHelper.DI;

namespace Util.AuthorizationHelper.Test;

/// <summary>
/// Authentication scheme names are the key schemes are registered under, so two standards can only coexist in one
/// host if they report different ones. These pin that, since the consequence of getting it wrong is a host that
/// refuses to start.
/// </summary>
public class ApiKeySchemeRegistrationTests
{
    /// <summary>A standard that does not name itself, so it inherits the shared base scheme name.</summary>
    private abstract class InheritsBaseSchemeName : SpecApiAuthStandardsV2026Base, ISpecApiAuthStandards
    {
        public static Func<string, string>? ApiKeyToClaimValueTransformer => null;
        public static string DefaultClaimType => "HasSomeOtherApiKey";
    }

    private static IReadOnlyList<string> RegisteredSchemeNames(Action<AuthenticationBuilder> register)
    {
        var services = new ServiceCollection();
        register(services.AddAuthentication());

        var options = services.BuildServiceProvider()
            .GetRequiredService<IOptions<AuthenticationOptions>>().Value;

        return options.Schemes.Select(x => x.Name).ToArray();
    }

    [Fact]
    public void Md5AndSha256StandardsCanBeRegisteredSideBySide()
    {
        var schemes = RegisteredSchemeNames(builder =>
        {
            builder.AddSpecialApiKeyAuthentication<SpecApiAuthStandardsV2026_Md5>();
            builder.AddSpecialApiKeyAuthentication<SpecApiAuthStandardsV2026_Sha256>();
        });

        Assert.Contains(SpecApiAuthStandardsV2026_Md5.DefaultSchemeName, schemes);
        Assert.Contains(SpecApiAuthStandardsV2026_Sha256.DefaultSchemeName, schemes);
    }

    /// <summary>
    /// The failure the sha256 standard's own scheme name exists to avoid: two standards inheriting the shared
    /// base name collide, and the host throws while building its authentication options.
    /// </summary>
    [Fact]
    public void TwoStandardsSharingTheBaseSchemeNameCollide()
    {
        Assert.Equal(SpecApiAuthStandardsV2026_Md5.DefaultSchemeName, InheritsBaseSchemeName.DefaultSchemeName);

        var error = Assert.Throws<InvalidOperationException>(() => RegisteredSchemeNames(builder =>
        {
            builder.AddSpecialApiKeyAuthentication<SpecApiAuthStandardsV2026_Md5>();
            builder.AddSpecialApiKeyAuthentication<InheritsBaseSchemeName>();
        }));

        Assert.Contains(SpecApiAuthStandardsV2026Base.MyDefaultSchemeName, error.Message);
    }

    /// <summary>
    /// Hiding a static member with <c>new</c> would be a trap if anything resolved it through the base type, so
    /// this pins that every path the library actually uses goes through the generic parameter and therefore sees
    /// the derived value. The base still reports its own name - that is the hazard, and it is why this is pinned.
    /// </summary>
    [Fact]
    public void Sha256SchemeNameResolvesThroughTheGenericParameter()
    {
        Assert.Equal("ApiKeySha256", SchemeNameOf<SpecApiAuthStandardsV2026_Sha256>());
        Assert.Equal("ApiKey", SchemeNameOf<SpecApiAuthStandardsV2026_Md5>());

        // the inherited member is still visible on the base, which is the wart this test documents
        Assert.Equal("ApiKey", SpecApiAuthStandardsV2026Base.DefaultSchemeName);
    }

    /// <summary>The identity a key produces must carry the derived scheme name, or claim extraction filters it out.</summary>
    [Fact]
    public void Sha256IdentityCarriesItsOwnSchemeName()
    {
        var claim = ISpecApiAuthStandards.StandardizedApiKeyClaim<SpecApiAuthStandardsV2026_Sha256>("some-key");

        Assert.Equal(SpecApiAuthStandardsV2026_Sha256.MyDefaultClaimType, claim.ClaimType);
        Assert.NotEqual(SpecApiAuthStandardsV2026_Md5.MyDefaultClaimType, claim.ClaimType);
    }

    private static string SchemeNameOf<T>() where T : ISpecApiAuthStandards => T.DefaultSchemeName;
}
