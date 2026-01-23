namespace Util.AuthorizationHelper.Policies;

/// <summary>
/// Projects that use predefined policy sets should define an abstract class implementing this interface
/// Mainly serves to ensure the presence of the static property ContainedPolicyDefinitions, for consistency / clean code
/// </summary>
public interface IPolicyDefinitionContainer
{
    /// <summary>
    /// All policy definitions in this container.
    /// </summary>
    IReadOnlyCollection<InternalPolicyDefinition> ContainedPolicyDefinitions { get; }
}