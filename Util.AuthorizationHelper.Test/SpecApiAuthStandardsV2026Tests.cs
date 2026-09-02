using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Util.AuthorizationHelper.Authentication;

namespace Util.AuthorizationHelper.Test;

/// <summary>
/// Base test class for all ISpecApiAuthStandards versions
/// </summary>
/// <typeparam name="TStandard"></typeparam>
public abstract class SpecApiAuthStandardsV2026_TestsBase<TStandard>
    where TStandard : ISpecApiAuthStandards
{
    protected string TestApiKey = "blaZaraBla123";
    protected string ExpectedHeader => TStandard.DefaultHeaderName;

    protected string WrongHeader => $"wrong_{ExpectedHeader}";
    protected string ExpectedClaimType => TStandard.DefaultClaimType;

    protected string ExpectedSchemeName => TStandard.DefaultSchemeName;
    protected abstract string ExpectedClaimValue { get; }

    [Fact]
    public void CreatesCorrectStandardizedApiKeyClaim()
    {
        var res = ISpecApiAuthStandards.StandardizedApiKeyClaim<TStandard>(TestApiKey);

        Assert.Equal(ExpectedClaimType, res.ClaimType);
        Assert.Equal(ExpectedClaimValue, res.Value);
    }

    [Fact]
    public void CreatesCorrectAuthTicket()
    {
        var request = new DefaultHttpRequest(new DefaultHttpContext())
        {
            Headers = { [ExpectedHeader] = TestApiKey }
        };

        var ticket = SpecApiAuthHandler<TStandard>.GetAuthenticationTicketFromRequest(request, NullLogger.Instance);
        
        Assert.NotNull(ticket);
        Assert.Equal(ExpectedSchemeName, ticket.AuthenticationScheme);
        
        var identities = ticket.Principal.Identities.ToArray();
        Assert.Single(identities);
        Assert.Equal(ExpectedSchemeName, identities.Single().AuthenticationType);

        var claims = identities.Single().Claims.ToArray();
        Assert.Single(claims);

        var res = claims.Single();

        Assert.Equal(ExpectedClaimType, res.Type);
        Assert.Equal(ExpectedClaimValue, res.Value);
    }

    [Fact]
    public void NoAuthWithWrongHeader()
    {
        var request = new DefaultHttpRequest(new DefaultHttpContext())
        {
            Headers = { [WrongHeader] = TestApiKey }
        };

        var ticket = SpecApiAuthHandler<TStandard>.GetAuthenticationTicketFromRequest(request, NullLogger.Instance);

        Assert.Null(ticket);
    }


}

public class SpecApiAuthStandardsV2026_Direct_Tests : SpecApiAuthStandardsV2026_TestsBase<SpecApiAuthStandardsV2026_Direct>
{
    /// <inheritdoc />
    protected override string ExpectedClaimValue => TestApiKey;
}

public class SpecApiAuthStandardsV2026_Md5_Tests : SpecApiAuthStandardsV2026_TestsBase<SpecApiAuthStandardsV2026_Md5>
{
    private const string Md5HashOfTestApiKey = "4111d3bd7d3bd2842ad0e84b77290e0f";

    /// <inheritdoc />
    protected override string ExpectedClaimValue => Md5HashOfTestApiKey;
}


//public class SpecApiAuthStandardsV2026_MD5_Tests
//{
//    [Fact]
//    public void SpecApiAuthStandardsV2026_Md5_TransformsCorrectly()
//    {
//        var apiKey = "blaZaraBla123";
//        var apiKeyHash = "4111d3bd7d3bd2842ad0e84b77290e0f";
//        var expClaimValue = apiKeyHash;

//        var res = ISpecApiAuthStandards.StandardizedApiKeyClaim<SpecApiAuthStandardsV2026_Md5>(apiKey);

//        Assert.Equal(Authentication.SpecApiAuthStandardsV2026_Md5.MyDefaultClaimType, res.ClaimType);
//        Assert.Equal(expClaimValue, res.Value);
//    }

//    [Fact]
//    public void SpecApiAuthStandardsV2026_Direct_TransformsCorrectly()
//    {
//        var apiKey = "blaZaraBla123";
//        var apiKeyHash = "4111d3bd7d3bd2842ad0e84b77290e0f";
//        var expClaimValue = apiKey;

//        var res = ISpecApiAuthStandards.StandardizedApiKeyClaim<SpecApiAuthStandardsV2026_Direct>(apiKey);

//        Assert.Equal(Authentication.SpecApiAuthStandardsV2026_Direct.MyDefaultClaimType, res.ClaimType);
//        Assert.Equal(expClaimValue, res.Value);
//    }
//}