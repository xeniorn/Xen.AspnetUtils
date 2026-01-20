namespace Util.AuthorizationHelper.DI.ConfigSupport.Base;

/// <summary>
/// Base interface for all configuration mappers that link an input type to associated permissions, which is also aware of the true item type the config represents and can construct it
/// </summary>
/// <typeparam name="TConfigurationEntry"></typeparam>
/// <typeparam name="TItem"></typeparam>
public interface IItemToInternalPermissionMapConfiguration<TConfigurationEntry, TItem> 
    : IItemToInternalPermissionMapConfiguration<TConfigurationEntry>
    where TConfigurationEntry : IPermissionsContainer
{
    TItem ToItem(TConfigurationEntry configurationEntry);
}

/// <summary>
/// Base interface for all configuration mappers that link an input type to associated permissions
/// </summary>
/// <typeparam name="TConfigurationEntry"></typeparam>
public interface IItemToInternalPermissionMapConfiguration<TConfigurationEntry>
    where TConfigurationEntry : IPermissionsContainer
{
    TConfigurationEntry[] Entries { get; set; }
}