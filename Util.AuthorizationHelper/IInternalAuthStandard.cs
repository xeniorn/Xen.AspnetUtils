using Util.AuthorizationHelper.Claims;
using Util.AuthorizationHelper.Authorization;
using Util.AuthorizationHelper.Policies;

namespace Util.AuthorizationHelper;

/// <summary>
/// Marker interface for app-internal claim sets.
/// Used to constraint the generic parameter of <see cref="AppInternalClaimDefinition{T}"/> and other items. Ensures that there's a unique namespace per claim set, and that all elements adhere to the same standard
/// Apps are expected to define their own version of:
/// <list type="bullet">
/// <item><see cref="AppInternalClaimDefinition{T}"/></item>
/// <item><see cref="InternalIdentityGeneratorBase{T}"/></item>
/// </list>
/// ...and likely also the various helpers:
/// <list type="bullet">
/// <item><see cref="AdminOverrideAuthorizationHandler{T}"/></item> 
/// </list>
/// It's designed to work well together with a <see cref="IStaticPolicyDefinitionContainer"/> defining policies 
/// </summary>
public interface IInternalAuthStandard
{
    /// <summary>
    /// Declared type string of the Identity used by this claim set
    /// </summary>
    public static abstract string AuthenticationTypeName { get; }
    public static abstract string Namespace { get; }

    /// <summary>
    /// Special definition that can be used to identify admin-like claims in this claim set
    /// Null if such a claim does not exist
    /// </summary>
    public static abstract ClaimDefinition? SpecialAdminLikeClaim { get; }
    public static abstract Type MyType { get; }
}