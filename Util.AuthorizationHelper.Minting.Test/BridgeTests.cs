using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Util.ApiKeyMinting;
using Util.AuthorizationHelper.Authentication;

namespace Util.AuthorizationHelper.Minting.Test;

/// <summary>
/// The stored hash must equal what the authentication handler will emit for the same key. If these ever diverge,
/// minted keys authenticate perfectly and then match nothing - a failure that looks nothing like its cause, which
/// is why it is pinned here rather than left to integration testing.
/// </summary>
public class StandardApiKeyHasherTests
{
    private static string ClaimValueFromHandler<TStandard>(string apiKey)
        where TStandard : ISpecApiAuthStandards
    {
        var request = new DefaultHttpRequest(new DefaultHttpContext())
        {
            Headers = { [TStandard.DefaultHeaderName] = apiKey }
        };

        var ticket = SpecApiAuthHandler<TStandard>.GetAuthenticationTicketFromRequest(request, NullLogger.Instance);

        return ticket!.Principal.Identities.Single().Claims.Single().Value;
    }

    [Fact]
    public void Sha256_HashMatchesWhatTheHandlerEmits()
    {
        var apiKey = new Base64UrlApiKeyGenerator("ptk_").Generate();

        var stored = new StandardApiKeyHasher<SpecApiAuthStandardsV2026_Sha256>().Hash(apiKey);

        Assert.Equal(ClaimValueFromHandler<SpecApiAuthStandardsV2026_Sha256>(apiKey), stored);
    }

    [Fact]
    public void Md5_HashMatchesWhatTheHandlerEmits()
    {
        var apiKey = new Base64UrlApiKeyGenerator("ptk_").Generate();

        var stored = new StandardApiKeyHasher<SpecApiAuthStandardsV2026_Md5>().Hash(apiKey);

        Assert.Equal(ClaimValueFromHandler<SpecApiAuthStandardsV2026_Md5>(apiKey), stored);
    }

    /// <summary>
    /// The algorithm id is stored on every key so two standards can coexist; it has to actually distinguish them.
    /// </summary>
    [Fact]
    public void AlgorithmIdDistinguishesTheStandards()
    {
        Assert.NotEqual(
            new StandardApiKeyHasher<SpecApiAuthStandardsV2026_Md5>().AlgorithmId,
            new StandardApiKeyHasher<SpecApiAuthStandardsV2026_Sha256>().AlgorithmId);
    }

    [Fact]
    public void GeneratedKeysAreDistinctAndCarryThePrefix()
    {
        var generator = new Base64UrlApiKeyGenerator("ptk_");

        var keys = Enumerable.Range(0, 100).Select(_ => generator.Generate()).ToArray();

        Assert.All(keys, x => Assert.StartsWith("ptk_", x));
        Assert.Equal(100, keys.Distinct().Count());
    }
}

public class PolicyRequirementCatalogTests
{
    private readonly PolicyRequirementCatalog<TestAuthStandard, TestPolicySet> _sut = new();

    [Fact]
    public void PolicyResolvesToItsPermission()
    {
        Assert.True(_sut.TryGetRequirementAlternatives("CanViewBillingInfo", out var alternatives));

        Assert.Equal(["CanViewBillingInfo"], Assert.Single(alternatives));
    }

    /// <summary>
    /// An "allow anonymous" policy is one alternative containing no claims - not zero alternatives. The
    /// distinction survives the translation, because the resolver treats the two differently.
    /// </summary>
    [Fact]
    public void AllowAnonymous_IsOneAlternativeThatRequiresNothing()
    {
        Assert.True(_sut.TryGetRequirementAlternatives("AllowAnonymous", out var alternatives));

        Assert.Empty(Assert.Single(alternatives));
    }

    [Fact]
    public void ClaimsFromAnotherNamespace_AreNotOfferedAsPermissions()
    {
        Assert.True(_sut.TryGetRequirementAlternatives("ForeignClaimPolicy", out var alternatives));

        Assert.Empty(Assert.Single(alternatives));
        Assert.DoesNotContain("SomethingElse", _sut.KnownPermissions);
    }

    [Fact]
    public void KnownPermissionsCoverTheDeclaredPolicies()
    {
        Assert.Contains("CanViewBillingInfo", _sut.KnownPermissions);
        Assert.Contains("CanEditBillingInfo", _sut.KnownPermissions);
    }

    [Fact]
    public void UnknownPolicy_IsReportedRatherThanThrown()
    {
        Assert.False(_sut.TryGetRequirementAlternatives("NoSuchPolicy", out _));
    }

    [Fact]
    public void PermissionRoundTripsThroughItsClaim()
    {
        var claim = InternalPermissionClaims<TestAuthStandard>.ToClaim("CanViewBillingInfo");

        Assert.Equal(TestAuthStandard.Namespace, claim.ClaimType);
        Assert.Equal("CanViewBillingInfo", InternalPermissionClaims<TestAuthStandard>.ToPermissionOrNull(claim));
    }
}

public class SubjectSelectorExtractorTests
{
    private static ClaimsPrincipal PrincipalWith(params (string Type, string Value)[] claims)
        => new(new ClaimsIdentity(claims.Select(x => new Claim(x.Type, x.Value)), "test"));

    [Fact]
    public void RolesAreExtractedByDefault()
    {
        var subjects = new SubjectSelectorExtractor().Extract(
            PrincipalWith(("roles", "Billing"), ("roles", "Instruments")));

        Assert.Equal(
            [new SubjectSelector("roles", "Billing"), new SubjectSelector("roles", "Instruments")],
            subjects);
    }

    /// <summary>
    /// Without the allow-list, a grant could be written against something every caller carries - an audience or an
    /// issuer - and would then be held by everyone.
    /// </summary>
    [Fact]
    public void ClaimTypesOutsideTheAllowListAreIgnored()
    {
        var subjects = new SubjectSelectorExtractor().Extract(
            PrincipalWith(("roles", "Billing"), ("aud", "api://something"), ("iss", "https://login")));

        Assert.Equal([new SubjectSelector("roles", "Billing")], subjects);
    }

    [Fact]
    public void AdditionalClaimTypesCanBeAllowed()
    {
        var options = new SubjectSelectorExtractor.MyOptions { SubjectClaimTypes = ["roles", "preferred_username"] };

        var subjects = new SubjectSelectorExtractor(options).Extract(
            PrincipalWith(("preferred_username", "someone@example.org"), ("aud", "ignored")));

        Assert.Equal([new SubjectSelector("preferred_username", "someone@example.org")], subjects);
    }

    [Fact]
    public void HoldsIsTrueOnlyForSubjectsThePrincipalActuallyCarries()
    {
        var extractor = new SubjectSelectorExtractor();
        var principal = PrincipalWith(("roles", "Billing"));

        Assert.True(extractor.Holds(principal, new SubjectSelector("roles", "Billing")));
        Assert.False(extractor.Holds(principal, new SubjectSelector("roles", "Instruments")));
    }
}
