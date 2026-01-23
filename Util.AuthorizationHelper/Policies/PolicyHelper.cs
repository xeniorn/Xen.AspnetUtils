using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Util.AuthorizationHelper.Claims;
using Util.AuthorizationHelper.Principals;

namespace Util.AuthorizationHelper.Policies;

/// <summary>
/// Extension methods for less verbose setup of internal policies and claims
/// </summary>
public static class PolicyHelper
{
    /// <summary>
    /// Strongly typed version of <see cref="AuthorizationPolicyBuilder.RequireClaim(string, string[])"/>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="claimDefinition"></param>
    /// <returns></returns>
    public static AuthorizationPolicyBuilder RequireClaim(this AuthorizationPolicyBuilder builder, ClaimDefinition claimDefinition)
    {
        return builder.RequireClaim(claimDefinition.ClaimType, claimDefinition.Value);
    }

    /// <summary>
    /// Convenience method to add multiple policies at once
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="policyDefinitions"></param>
    /// <param name="authSchemes"></param>
    public static void AddPolicies(this AuthorizationOptions builder, IReadOnlyCollection<InternalPolicyDefinition> policyDefinitions, IReadOnlyCollection<string>? authSchemes = null)
    {
        foreach (var x in policyDefinitions)
        {
            builder.AddPolicy(x, authSchemes);
        }
    }

    /// <summary>
    /// Strongly typed alternative to <see cref="AuthorizationOptions.AddPolicy(string, Action{AuthorizationPolicyBuilder})"/>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="policyDefinition"></param>
    /// <param name="authSchemes">which auth schemes are allowed to participate. Otherwise, use default</param>
    // ReSharper disable once MemberCanBePrivate.Global
    public static void AddPolicy(this AuthorizationOptions builder, InternalPolicyDefinition policyDefinition, IReadOnlyCollection<string>? authSchemes = null)
     => builder.AddPolicy(
         policyDefinition.Name, 
         policyBuilder => policyBuilder.SetupUsing(policyDefinition, authSchemes));

    /// <summary>
    /// Extracted for easier testing / debugging purposes
    /// </summary>
    /// <param name="policyBuilder"></param>
    /// <param name="policyDefinition"></param>
    /// <param name="authSchemes"></param>
    internal static void SetupUsing(this AuthorizationPolicyBuilder policyBuilder, 
        InternalPolicyDefinition policyDefinition, 
        IReadOnlyCollection<string>? authSchemes = null)
    {
        if (authSchemes is not null)
        {
            policyBuilder.AuthenticationSchemes = authSchemes.ToArray();
        }

        policyBuilder
            .RequireAssertion(context =>
            {
                foreach (var claimRequirement in policyDefinition.ClaimRequirementAlternatives)
                {
                    if (context.User.HasClaims(claimRequirement.RequiredClaims))
                        return true;
                }

                return false;
            });

        
    }
    
}