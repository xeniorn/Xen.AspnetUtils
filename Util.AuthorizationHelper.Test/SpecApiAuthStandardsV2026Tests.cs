using Util.AuthorizationHelper.Authentication;

namespace Util.AuthorizationHelper.Test;

public class SpecApiAuthStandardsV2026Tests
{
    [Fact]
    public void SpecApiAuthStandardsV2026_Md5_TransformsCorrectly()
    {
        var apiKey = "blaZaraBla123";
        var apiKeyHash = "4111d3bd7d3bd2842ad0e84b77290e0f";
        var expClaimValue = apiKeyHash;

        var res = ISpecApiAuthStandards.StandardizedApiKeyClaim<SpecApiAuthStandardsV2026_Md5>(apiKey);

        Assert.Equal(Authentication.SpecApiAuthStandardsV2026_Md5.MyDefaultClaimType, res.ClaimType);
        Assert.Equal(expClaimValue, res.Value);
    }

    [Fact]
    public void SpecApiAuthStandardsV2026_Direct_TransformsCorrectly()
    {
        var apiKey = "blaZaraBla123";
        var apiKeyHash = "4111d3bd7d3bd2842ad0e84b77290e0f";
        var expClaimValue = apiKey;

        var res = ISpecApiAuthStandards.StandardizedApiKeyClaim<SpecApiAuthStandardsV2026_Direct>(apiKey);

        Assert.Equal(Authentication.SpecApiAuthStandardsV2026_Direct.MyDefaultClaimType, res.ClaimType);
        Assert.Equal(expClaimValue, res.Value);
    }
}