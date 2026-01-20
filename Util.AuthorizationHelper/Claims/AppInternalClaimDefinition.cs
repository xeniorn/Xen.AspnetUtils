namespace Util.AuthorizationHelper.Claims;

/// <summary>
/// Supposed to be inherited by app-internal claim definition type (1 per security domain)
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract record AppInternalClaimDefinition<T> : ClaimDefinition
    where T : IInternalAuthStandard
{
    private const int MaxLength = 500;
    protected AppInternalClaimDefinition(string name) : base(T.Namespace, name)
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