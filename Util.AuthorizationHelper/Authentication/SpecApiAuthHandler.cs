using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Util.AuthorizationHelper.Authentication;

/// <summary>
/// Handler that adds a separate identity per provided API key according to the chosen standard <see cref="ISpecApiAuthStandards"/>,
/// with a single claim each that asserts the holder "has" the given API key or a derived value.
/// Purpose of this is to enable claim/policy based authorization based on API keys provided in HTTP request headers
/// </summary>
public class SpecApiAuthHandler<TApiKeyStandard> : AuthenticationHandler<SpecApiAuthHandler<TApiKeyStandard>.MyOptions>
    where TApiKeyStandard : ISpecApiAuthStandards
{
    public class MyOptions : AuthenticationSchemeOptions
    {
        public string HeaderNameThatContainsApiKey { get; set; } = TApiKeyStandard.DefaultHeaderName;
        public string HasApiClaimType { get; set; } = TApiKeyStandard.DefaultClaimType;
    }


    /// <inheritdoc />
    public SpecApiAuthHandler(IOptionsMonitor<MyOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    /// <inheritdoc />
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderNameThatContainsApiKey, out var apiKeyValues))
            return AuthenticateResult.NoResult();

        var principal = new ClaimsPrincipal();

        foreach (var apiKeyValue in apiKeyValues.Where(x => !string.IsNullOrEmpty(x)).OfType<string>())
        {
            var identity = new ClaimsIdentity(Scheme.Name);
            identity.AddClaim(new Claim(Options.HasApiClaimType, apiKeyValue));
            principal.AddIdentity(identity);
        }

        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}