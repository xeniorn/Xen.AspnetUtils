using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Util.AuthorizationHelper.Claims.Extraction.Base;

namespace Util.AuthorizationHelper.Claims;

/// <summary>
/// An <see cref="IClaimsTransformation"/> that creates a <see cref="ClaimsIdentity"/> representing the internal auth state for this application, conforming the selected <see cref="IInternalAuthStandard"/>
/// This ClaimsIdentity will contain only claims of the type defined in the standard.
/// </summary>
/// <typeparam name="TStandard"></typeparam>
/// <param name="claimExtractors">Accepts a  typed collection of internal claim extractors to allow for easier injection</param>
/// <param name="options"></param>
public abstract class InternalIdentityGeneratorBase<TStandard>(IEnumerable<IInternalClaimExtractor<TStandard>> claimExtractors, InternalIdentityGeneratorBase<TStandard>.MyOptions? options)
    : IClaimsTransformation
    where TStandard : IInternalAuthStandard
{
    /// <summary>
    /// 
    /// </summary>
    protected MyOptions Options { get; } = options ?? new MyOptions();

    /// <summary>
    /// 
    /// </summary>
    public class MyOptions
    {
        /// <summary>
        /// What to do with other ClaimIdentities
        /// </summary>
        public OtherIdentityHandlingMode OtherIdentityHandling { get; set; } = OtherIdentityHandlingMode.Keep;
        /// <summary>
        /// What to do if the internal identity already exists
        /// </summary>
        public PreexistingInternalIdentityHandlingMode PreexistingInternalIdentityHandling { get; set; } = PreexistingInternalIdentityHandlingMode.Merge;

        public enum OtherIdentityHandlingMode
        {
            Keep,
            Remove
        }

        public enum PreexistingInternalIdentityHandlingMode
        {
            Merge,
            Ignore,
            Error
        }
    }

    public static class MyExceptions
    {
        public class ExistingInternalIdentityException : Exception
        {
            public ExistingInternalIdentityException()
                : base($"Preexisting internal identity of type {TStandard.AuthenticationTypeName} found while handling mode is set to disallow this.")
            {
            }
        }
    }

    /// <summary>
    /// All extractors configured for this generator instance
    /// </summary>
    protected IReadOnlyCollection<IInternalClaimExtractor> ClaimExtractors { get; } = (IReadOnlyCollection<IInternalClaimExtractor>)claimExtractors.ToArray();

    /// <summary>
    /// Creates a new <see cref="ClaimsIdentity"/> instance using the authentication type specified by
    /// <c>T.AuthenticationTypeName</c>.
    /// </summary>
    /// <returns>A <see cref="ClaimsIdentity"/> initialized with the authentication type from <c>T.AuthenticationTypeName</c>.</returns>
    private ClaimsIdentity ConstructInternalIdentity() => new ClaimsIdentity(TStandard.AuthenticationTypeName);

    /// <summary>
    /// Obtains the internal ClaimsIdentity according to the configured handling mode.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    /// <exception cref="MyExceptions.ExistingInternalIdentityException"></exception>
    /// <exception cref="UnreachableException"></exception>
    private ClaimsIdentity GetInternalIdentity(ClaimsPrincipal principal)
    {
        if (Options.PreexistingInternalIdentityHandling == MyOptions.PreexistingInternalIdentityHandlingMode.Ignore)
            return ConstructInternalIdentity();

        var existing = principal.Identities
            .Where(identity => identity.AuthenticationType == TStandard.AuthenticationTypeName)
            .ToArray();

        if (Options.PreexistingInternalIdentityHandling == MyOptions.PreexistingInternalIdentityHandlingMode.Error && existing.Any())
            throw new MyExceptions.ExistingInternalIdentityException();

        if (Options.PreexistingInternalIdentityHandling == MyOptions.PreexistingInternalIdentityHandlingMode.Merge)
        {
            var newInternalIdentity = ConstructInternalIdentity();
            foreach (var identity in existing)
            {
                foreach (var claim in identity.Claims)
                {
                    newInternalIdentity.AddClaim(claim);
                }
            }

            return newInternalIdentity;
        }

        throw new UnreachableException("This should not be possible since all options are exhausted");
    }

    /// <inheritdoc />
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var internalIdentity = GetInternalIdentity(principal);

        var claimMap = new Dictionary<IInternalClaimExtractor, IReadOnlyCollection<ClaimDefinition>>();

        foreach (var extractor in ClaimExtractors)
        {
            claimMap[extractor] = await extractor.GetClaimDefinitions(principal);
        }

        var allClaims = claimMap
            .SelectMany(x => x.Value)
            .ToHashSet();
        
        internalIdentity.AddClaims(allClaims.Select(x => x.ToClaim()));

        var newPrincipal = new ClaimsPrincipal(internalIdentity);

        switch (Options.OtherIdentityHandling)
        {
            case MyOptions.OtherIdentityHandlingMode.Keep:
                foreach (var identity in principal.Identities)
                {
                    if (identity.AuthenticationType != TStandard.AuthenticationTypeName)
                    {
                        newPrincipal.AddIdentity(identity);
                    }
                }
                break;
            case MyOptions.OtherIdentityHandlingMode.Remove:
                // Do nothing, other identities are not added
                break;
            default:
                throw new UnreachableException("This should not be possible since all options are exhausted");
        }

        return newPrincipal;
    }
}