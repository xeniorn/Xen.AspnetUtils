using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace Util.AuthorizationHelper.Authentication;

/// <summary>
/// Handler that adds a separate identity per provided API key according to the chosen standard <see cref="ISpecApiAuthStandards"/>,
/// with a single claim each that asserts the holder "has" the given API key or a derived value.
/// Purpose of this is to enable claim/policy based authorization based on API keys provided in HTTP request headers
/// </summary>
public class SpecApiAuthHandler<TApiKeyStandard> : AuthenticationHandler<AuthenticationSchemeOptions>
    where TApiKeyStandard : ISpecApiAuthStandards
{
    /// <inheritdoc />
    public SpecApiAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    /// <inheritdoc />
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = GetAuthenticationTicketFromRequest(Request, Logger ?? NullLogger.Instance) is { } ticket
            ? AuthenticateResult.Success(ticket)
            : AuthenticateResult.NoResult();

        return Task.FromResult(result);
    }

    /// <summary>
    /// Does the whole process from Request to AuthTicket. Separated from <see cref="HandleAuthenticateAsync"/>
    /// to allow for easier unit testing without having to mock the whole auth framework
    /// </summary>
    /// <param name="request"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    internal static AuthenticationTicket? GetAuthenticationTicketFromRequest(HttpRequest request, ILogger? logger = null)
    {
        if (!request.Headers.TryGetValue(TApiKeyStandard.DefaultHeaderName, out var apiKeyValues))
            return null;

        var principal = new ClaimsPrincipal();
        
        foreach (var apiKeyValue in apiKeyValues.Where(x => !string.IsNullOrEmpty(x)).OfType<string>())
        {
            // one claim per digest the standard declares. A standard supporting several lets key stores built on
            // different digests be matched from a single scheme - a stored hash can never be recomputed into
            // another digest, so the alternative would be reissuing every existing key
            var claimDefs = ISpecApiAuthStandards.StandardizedApiKeyClaims<TApiKeyStandard>(apiKeyValue);

            var identity = new ClaimsIdentity(TApiKeyStandard.DefaultSchemeName);

            foreach (var claimDef in claimDefs)
            {
                identity.AddClaim(claimDef.ToClaim());
            }

            principal.AddIdentity(identity);

            if (logger is not null && logger.IsEnabled(LogLevel.Debug))
            {
                logger.Log(LogLevel.Debug, "Added a new identity ({identity}) with claims {claimDefs}", identity.Name, claimDefs);
            }
        }

        var ticket = new AuthenticationTicket(principal, TApiKeyStandard.DefaultSchemeName);
        return ticket;
    }
}