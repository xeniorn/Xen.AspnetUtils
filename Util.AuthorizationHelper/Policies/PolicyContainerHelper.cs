namespace Util.AuthorizationHelper.Policies;

/// <summary>
/// Provides helper methods for retrieving internal policy definitions from a specified type, which is supposed to be a <see cref="IStaticPolicyDefinitionContainer"/>.
/// </summary>
/// <remarks>This class is intended for use with types that expose static fields of type InternalPolicyDefinition.
/// All members are static and thread safe.</remarks>
public static class PolicyContainerHelper
{
    /// <summary>
    /// Retrieves all public static fields of the specified type that are assignable to InternalPolicyDefinition. Works also for types that are not a <see cref="IStaticPolicyDefinitionContainer"/> !.
    /// </summary>
    /// <remarks>Only fields that are public, static, and assignable to InternalPolicyDefinition are included.
    /// Inherited fields are also considered.</remarks>
    /// <param name="type">The type to inspect for public static fields of type InternalPolicyDefinition.</param>
    /// <returns>A read-only collection containing all InternalPolicyDefinition instances found in the specified type. The
    /// collection is empty if no matching fields are found.</returns>
    public static IReadOnlyCollection<InternalPolicyDefinition> GetAllInternalPolicyDefinitionsInType(Type type) => type
        .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
        .Where(f => f.FieldType.IsAssignableTo(typeof(InternalPolicyDefinition)))
        .Select(f => (InternalPolicyDefinition)f.GetValue(null)!)
        .ToArray();
    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IReadOnlyCollection<InternalPolicyDefinition> GetAllInternalPolicyDefinitionsInType<T>() where T: IStaticPolicyDefinitionContainer => GetAllInternalPolicyDefinitionsInType(typeof(T));
}