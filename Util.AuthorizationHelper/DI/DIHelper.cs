using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Util.AuthorizationHelper.Authentication;
using Util.AuthorizationHelper.Policies;

namespace Util.AuthorizationHelper.DI;

/// <summary>
/// Util class for injection of auth services
/// </summary>
public static class DIHelper
{
    /// <summary>
    /// Adds the special api key authentication that converts ApiKeys from the HTTP Request (typically in the header) into special single-claim identities, where the single claim is the possession of the API key in question
    /// The authentication type and claim type are defined by the provided <typeparamref name="TApiKeyStandard"></typeparamref>
    /// </summary>
    /// <typeparam name="TApiKeyStandard"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddSpecialApiKeyAuthentication<TApiKeyStandard>(this AuthenticationBuilder builder)
        where TApiKeyStandard : ISpecApiAuthStandards
    {
        return builder.AddScheme<AuthenticationSchemeOptions, SpecApiAuthHandler<TApiKeyStandard>>
        (
            TApiKeyStandard.DefaultSchemeName,
            options =>
            {
                // TODO: configure options here if needed
                return;
            }
        );
    }

    /// <summary>
    /// Adds the special api key authentication that converts ApiKeys from the HTTP Request (typically in the header) into special single-claim identities, where the single claim is the possession of the API key in question
    /// The authentication type and claim type are defined by the provided <typeparamref name="TApiKeyStandard"></typeparamref>
    /// </summary>
    /// <typeparam name="TApiKeyStandard"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static void AddSpecialApiKeyAuthentication<TApiKeyStandard>(this IHostApplicationBuilder builder)
        where TApiKeyStandard : ISpecApiAuthStandards
    {
        builder.Services.AddAuthentication()
            .AddSpecialApiKeyAuthentication<TApiKeyStandard>();
    }

    /// <summary>
    /// Adds authorization based on internal policies, using any of registered authentication schemes
    /// Auth schemes can be filtered using the <see cref="authenticationSchemeFilter"/> parameter
    /// </summary>
    /// <typeparam name="TPolicyDefinitionContainer"></typeparam>
    /// <param name="builder"></param>
    /// <param name="requireAuthenticationByDefault">If true, fallback policy will be set to default policy</param>
    /// <param name="authenticationSchemeFilter"></param>
    /// <returns><see cref="OptionsBuilder{AuthorizationOptions}"/>, which can be used to further configure the options</returns>
    public static OptionsBuilder<AuthorizationOptions> AddAppInternalAuthorizationUsingAllAuthenticationSchemes<TPolicyDefinitionContainer>(
        this IHostApplicationBuilder builder,
        bool requireAuthenticationByDefault = false,
        Func<AuthenticationScheme, bool>? authenticationSchemeFilter = null)
        where TPolicyDefinitionContainer : IStaticPolicyDefinitionContainer
    {
        builder.Services.AddAuthorizationCore();

        var ob = builder.Services.AddOptions<AuthorizationOptions>()
            .Configure<IAuthenticationSchemeProvider>((options, schemeProvider) =>
            {
                var allSchemes = schemeProvider.GetAllSchemesAsync()
                    .GetAwaiter().GetResult();

                var finalSchemes = authenticationSchemeFilter is not { } filter
                    ? allSchemes
                    : allSchemes.Where(filter);

                var finalSchemeNames = finalSchemes
                    .Select(x => x.Name)
                    .ToArray();

                var defaultPolicy =
                    new AuthorizationPolicy([new DenyAnonymousAuthorizationRequirement()], finalSchemeNames);

                // when [Authorize] is present without policy name
                options.DefaultPolicy = defaultPolicy;

                // when no [Authorize] nor [AllowAnonymous] is present
                if (requireAuthenticationByDefault)
                    options.FallbackPolicy = defaultPolicy;
                
#if DEBUG
                options.AddPolicy("TestPolicy", apb =>
                {
                    apb.AuthenticationSchemes = finalSchemeNames;
                    //apb.AuthenticationSchemes = ["Bearer"];
                    apb.RequireAuthenticatedUser();
                });
#endif

                var allPresetPolicies = TPolicyDefinitionContainer.ContainedPolicyDefinitions;
                options.AddPolicies(allPresetPolicies, finalSchemeNames);
            });

        return ob;
    }
}

