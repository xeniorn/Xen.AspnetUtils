using System.Security.Claims;
using Util.AuthorizationHelper.Authentication;
using Util.AuthorizationHelper.Claims;
using Util.AuthorizationHelper.Claims.Extraction.Base.ApiKey;

namespace Util.AuthorizationHelper.Test;

public class SpecApiKeyMapBasedClaimExtractor_TApiAuthStandardTests
{
    private class MyClaimSet : IInternalAuthStandard
    {
        /// <inheritdoc />
        public static string AuthenticationTypeName => $"{nameof(MyClaimSet)}_{nameof(AuthenticationTypeName)}";

        /// <inheritdoc />
        public static string Namespace => $"{nameof(MyClaimSet)}_{nameof(Namespace)}";

        /// <inheritdoc />
        public static ClaimDefinition? SpecialAdminLikeClaim => null;

        /// <inheritdoc />
        public static Type MyType => typeof(MyClaimSet);
    }

    private class MyMd5ApiKeyExtractor(
        SpecApiKeyMapBasedClaimExtractor<MyClaimSet, SpecApiAuthStandardsV2026_Md5>.GenericMyOptions options)
        : SpecApiKeyMapBasedClaimExtractor<MyClaimSet, SpecApiAuthStandardsV2026_Md5>(options)
    {
    }

    /// <summary>
    /// Confirms that a <see cref="SpecApiKeyMapBasedClaimExtractor{MyClaimSet, SpecApiAuthStandardsV2026_Md5}" /> correctly translates claims according to its config
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SpecApiKeyMapBasedClaimExtractor_WorksCorrectlyFor_SpecApiAuthStandardsV2026_Md5()
    {
        var apiKey = "myApiKey";
        var apiKeyHash = "c2309710a74537387de7eca2bb5305c6";

        var apiKeyClaim = new Claim(SpecApiAuthStandardsV2026_Md5.MyDefaultClaimType, apiKeyHash);

        var appInternalClaimValue = "MyInternalClaim";
        var appInternalClaim = new ClaimDefinition(MyClaimSet.Namespace, appInternalClaimValue);

        var origValue = "";
        
        var options =
            new SpecApiKeyMapBasedClaimExtractor<MyClaimSet, SpecApiAuthStandardsV2026_Md5>.GenericMyOptions()
            {
                ApiKeyAssociatedValueToClaimsMap = new Dictionary<string, ClaimDefinition[]>
                {
                    [apiKeyHash] = [appInternalClaim]
                }
            };

        var extractor = new MyMd5ApiKeyExtractor(options);

        var principal = new ClaimsPrincipal(new ClaimsIdentity([apiKeyClaim], SpecApiAuthStandardsV2026Base.MyDefaultSchemeName));

        var ident = await extractor.GetClaimDefinitions(principal, default);

        Assert.Single(ident);
        var derivedClaim = ident.Single();

        Assert.Equal(appInternalClaim.ClaimType, derivedClaim.ClaimType);
        Assert.Equal(appInternalClaim.Value, derivedClaim.Value);
    }

    [Fact]
    public async Task SpecApiKeyMapBasedClaimExtractorOptionsAreCorrectlyConfigured_Md5()
    {
        var origValue = "abcdef";
        var claims = new ClaimDefinition[] { new("mct", "mv") };

        var options =
            new SpecApiKeyMapBasedClaimExtractor<MyClaimSet, SpecApiAuthStandardsV2026_Md5>.GenericMyOptions()
            {
                ApiKeyAssociatedValueToClaimsMap = new Dictionary<string, ClaimDefinition[]>
                {
                    [origValue] = claims
                }
            };

        IClaimMappingContainer optionsAsAContainer = options;
        var sourceClaim1 = optionsAsAContainer.ClaimMapping.Single().SourceClaim;

        Assert.Equal(SpecApiAuthStandardsV2026_Md5.DefaultClaimType, sourceClaim1.ClaimType);
        Assert.Equal(origValue, sourceClaim1.Value);

        // not important here
        {
            var asFrozenDict = optionsAsAContainer.GetFrozenMapping();
            var sourceClaim2 = asFrozenDict.Single().Key;

            Assert.Equal(SpecApiAuthStandardsV2026_Md5.DefaultClaimType, sourceClaim2.ClaimType);
            Assert.Equal(origValue, sourceClaim2.Value);
        }
    }

    [Fact]
    public async Task SpecApiKeyMapBasedClaimExtractorOptionsAreCorrectlyConfigured_Direct()
    {
        var origValue = "abcdef";
        var claims = new ClaimDefinition[] { new("mct", "mv") };

        var options =
            new SpecApiKeyMapBasedClaimExtractor<MyClaimSet, SpecApiAuthStandardsV2026_Direct>.GenericMyOptions()
            {
                ApiKeyAssociatedValueToClaimsMap = new Dictionary<string, ClaimDefinition[]>
                {
                    [origValue] = claims
                }
            };

        IClaimMappingContainer optionsAsAContainer = options;
        var sourceClaim1 = optionsAsAContainer.ClaimMapping.Single().SourceClaim;

        Assert.Equal(SpecApiAuthStandardsV2026_Direct.DefaultClaimType, sourceClaim1.ClaimType);
        Assert.Equal(origValue, sourceClaim1.Value);

        // not important here
        {
            var asFrozenDict = optionsAsAContainer.GetFrozenMapping();
            var sourceClaim2 = asFrozenDict.Single().Key;

            Assert.Equal(SpecApiAuthStandardsV2026_Direct.DefaultClaimType, sourceClaim2.ClaimType);
            Assert.Equal(origValue, sourceClaim2.Value);
        }
    }
}