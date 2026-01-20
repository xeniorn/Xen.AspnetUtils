using System.Reflection;

namespace Util.AuthorizationHelper.Claims;

public static class ClaimHelper
{
    public static IReadOnlyCollection<AppInternalClaimDefinition<TAuthStandard>> AllHardcodedClaimsFromType<TAuthStandard>(
        Type typeContainingClaimDefinitions) where TAuthStandard : IInternalAuthStandard
    {
        //if (!internalAuthStandardType.IsAssignableTo(typeof(IInternalAuthStandard)))
        //{
        //    throw new ArgumentException($"Provided type must be assignable to {nameof(IInternalAuthStandard)}");
        //}
        
        var staticDefined = typeContainingClaimDefinitions
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.FieldType.IsAssignableTo(typeof(AppInternalClaimDefinition<TAuthStandard>)))
            .Select(f => (AppInternalClaimDefinition<TAuthStandard>)f.GetValue(null)!)
            .ToArray();

        
        AppInternalClaimDefinition<TAuthStandard>[] instanceDefined = [];

        try
        {
            var instance = Activator.CreateInstance(typeContainingClaimDefinitions);

            instanceDefined = instance is null
                ? []
                : typeContainingClaimDefinitions
                    .GetFields(System.Reflection.BindingFlags.Public | BindingFlags.Instance)
                    .Where(f => f.FieldType.IsAssignableTo(typeof(AppInternalClaimDefinition<TAuthStandard>)))
                    .Select(f => (AppInternalClaimDefinition<TAuthStandard>)f.GetValue(instance)!)
                    .ToArray();
        }
        catch
        {
            // ignore
        }

        return new HashSet<AppInternalClaimDefinition<TAuthStandard>>([
            ..staticDefined,
            ..instanceDefined
        ]);
    }


    public static IReadOnlyCollection<AppInternalClaimDefinition<TAuthStandard>> AllHardcodedClaims<TAuthStandard,
        TContainerType>() where TAuthStandard : IInternalAuthStandard
        => AllHardcodedClaimsFromType<TAuthStandard>(typeof(TContainerType));
}