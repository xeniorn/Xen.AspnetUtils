using System.Security.Claims;

namespace Util.AuthorizationHelper.Claims;

/// <summary>
/// Strongly typed definition of a single claim
/// </summary>
/// <param name="ClaimType"></param>
/// <param name="Value"></param>
public record ClaimDefinition(string ClaimType, string Value)
{
    // TODO: move to ext method, keep the record clean / free from deps
    public static ClaimDefinition FromClaim(Claim claim)
        => new ClaimDefinition(claim.Type, claim.Value);

    public Claim ToClaim(string? issuer = null)
    {
        return new Claim(ClaimType, Value, issuer);
    }
}