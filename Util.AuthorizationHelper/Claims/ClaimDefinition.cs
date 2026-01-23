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
    /// <summary>
    /// From standard framework Claim
    /// </summary>
    /// <param name="claim"></param>
    /// <returns></returns>
    public static ClaimDefinition FromClaim(Claim claim)
        => new ClaimDefinition(claim.Type, claim.Value);

    /// <summary>
    /// To standard framework Claim
    /// </summary>
    /// <param name="issuer"></param>
    /// <returns></returns>
    public Claim ToClaim(string? issuer = null)
    {
        return new Claim(ClaimType, Value, issuer);
    }
}