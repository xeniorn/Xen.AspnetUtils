namespace Util.AuthorizationHelper.DI.ConfigSupport.Base;

/// <summary>
/// Item is able to get or set a permissions array, compatible with DI/configuration
/// </summary>
public interface IPermissionsContainer
{
    string[] Permissions { get; set; }
};