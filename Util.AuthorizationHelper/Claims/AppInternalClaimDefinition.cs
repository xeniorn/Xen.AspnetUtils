namespace Util.AuthorizationHelper.Claims;

/// <summary>
/// Supposed to be inherited by app-internal claim definition type (1 per security domain)
/// </summary>
/// <typeparam name="TStandard"></typeparam>
public abstract record AppInternalClaimDefinition<TStandard> : ClaimDefinition
    where TStandard : IInternalAuthStandard
{
    private const int MaxLength = 500;
    protected AppInternalClaimDefinition(string name) : base(TStandard.Namespace, name)
    {
        ValidateName(name);
        Name = name;
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        if (name.Length > MaxLength)
            throw new ArgumentException(nameof(name));
    }

    public string Name { get; }

    
}