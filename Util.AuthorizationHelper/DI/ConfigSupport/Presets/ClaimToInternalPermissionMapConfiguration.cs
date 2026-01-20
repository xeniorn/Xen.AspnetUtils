using Util.AuthorizationHelper.Claims;
using Util.AuthorizationHelper.DI.ConfigSupport.Base;

namespace Util.AuthorizationHelper.DI.ConfigSupport.Presets;

/// <summary>
/// For mapping arbitrary claims to permissions in configuration (e.g. used for config of permissions from JWT)
/// </summary>
public record ClaimToInternalPermissionMapConfiguration : IItemToInternalPermissionMapConfiguration<ClaimToInternalPermissionMapConfiguration.ClaimToInternalPermissionMapConfigurationEntry, ClaimDefinition>
{
    public ClaimToInternalPermissionMapConfiguration.ClaimToInternalPermissionMapConfigurationEntry[] Entries { get; set; } = [];

    public ClaimDefinition ToItem(ClaimToInternalPermissionMapConfigurationEntry configurationEntry)
        => new ClaimDefinition(configurationEntry.SourceClaimType, configurationEntry.SourceClaimValue);

    /// <summary>
    /// 
    /// </summary>
    public record ClaimToInternalPermissionMapConfigurationEntry : IPermissionsContainer
    {
        public string SourceClaimType { get; set; } = string.Empty;
        public string SourceClaimValue { get; set; } = string.Empty;
        public string[] Permissions { get; set; } = [];
    };
};