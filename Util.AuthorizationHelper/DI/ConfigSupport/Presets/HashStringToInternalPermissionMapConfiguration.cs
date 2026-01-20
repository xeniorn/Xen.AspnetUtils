using Util.AuthorizationHelper.DI.ConfigSupport.Base;

namespace Util.AuthorizationHelper.DI.ConfigSupport.Presets;

/// <summary>
/// Used for mapping api keys to permissions without revealing the actual key in configuration, by using the md5 hash instead.
/// Hash is to be generated using the ascii encoding of the API key string as input
/// </summary>
public record HashStringToInternalPermissionMapConfiguration
    : IItemToInternalPermissionMapConfiguration<HashStringToInternalPermissionMapConfiguration.StringToInternalPermissionMapConfigurationEntry, string>
{

    /// <inheritdoc />
    public StringToInternalPermissionMapConfigurationEntry[] Entries { get; set; }

    public record StringToInternalPermissionMapConfigurationEntry : IPermissionsContainer
    {
        public string Hash { get; set; } = string.Empty;

        /// <inheritdoc />
        public string[] Permissions { get; set; } = [];
    }

    /// <inheritdoc />
    public string ToItem(StringToInternalPermissionMapConfigurationEntry configurationEntry)
        => configurationEntry.Hash;
}