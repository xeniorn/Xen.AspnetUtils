using Util.AuthorizationHelper.DI.ConfigSupport.Base;

namespace Util.AuthorizationHelper.DI.ConfigSupport.Presets;

/// <summary>
/// For mapping roles to permissions in configuration (e.g. used for config of permissions from JWT)
/// </summary>
public record RoleToInternalPermissionMapConfiguration : IItemToInternalPermissionMapConfiguration<RoleToInternalPermissionMapConfiguration.RoleToInternalPermissionMapConfigurationEntry, string>
{
    public static readonly string RoleClaimStringDefault = "roles";
    public string ExternalRoleClaimString { get; set; } = RoleClaimStringDefault;

    public RoleToInternalPermissionMapConfigurationEntry[] Entries { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    public record RoleToInternalPermissionMapConfigurationEntry : IPermissionsContainer
    {
        public string SourceRole { get; set; } = string.Empty;
        public string[] Permissions { get; set; } = [];
    };

    /// <inheritdoc />
    public string ToItem(RoleToInternalPermissionMapConfigurationEntry configurationEntry)
        => configurationEntry.SourceRole;
}