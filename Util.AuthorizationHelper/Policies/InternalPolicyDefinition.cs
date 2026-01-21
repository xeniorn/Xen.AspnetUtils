using Util.AuthorizationHelper.Claims;

namespace Util.AuthorizationHelper.Policies;



/// <summary>
/// Represents an internal policy definition that specifies a set of claim requirements and related metadata for
/// authorization purposes.
/// </summary>
/// <remarks>This type is intended for internal use to define authorization policies based on claims. Each policy
/// can specify one or more alternative sets of required claims, allowing for flexible authorization
/// scenarios.</remarks>
/// <param name="Name">The unique name of the policy definition.</param>
/// <param name="Description">A description of the policy's purpose or intended usage. May be empty if no description is provided.</param>
/// <param name="StrongerThanAdmin">true if the policy is considered stronger than the default administrator policy; otherwise, false.</param>
/// <param name="ClaimRequirementAlternatives">A collection of alternative claim requirements. The policy is satisfied if any one of the alternatives is met.
/// Cannot be null or empty.</param>
public record InternalPolicyDefinition(
    string Name,
    string Description,
    bool StrongerThanAdmin,
    IReadOnlyCollection<InternalPolicyDefinition.ClaimRequirement> ClaimRequirementAlternatives)
{
    public InternalPolicyDefinition(string name,
        ClaimDefinition singleClaimRequirement)
        : this(name, string.Empty, false, [new(singleClaimRequirement)])
    {
    }

    public InternalPolicyDefinition(string name,
        string description,
        ClaimDefinition singleClaimRequirement)
        : this(name, description, false, [new(singleClaimRequirement)])
    {
    }

    public InternalPolicyDefinition(string name,
        ClaimRequirement claimRequirement)
        : this(name, string.Empty, false, [claimRequirement])
    {
    }

    public InternalPolicyDefinition(string name,
        string description,
        ClaimRequirement claimRequirement)
        : this(name, description, false, [claimRequirement])
    {
    }

    public InternalPolicyDefinition(string name,
        string Description,
        params ClaimRequirement[] claimRequirementAlternatives)
        : this(name, Description, false, claimRequirementAlternatives)
    {
    }

    public record ClaimRequirement(IReadOnlyCollection<ClaimDefinition> RequiredClaims)
    {
        public ClaimRequirement(string claimNamespace, string claimValue) : this([new(claimNamespace, claimValue)]) { }
        public ClaimRequirement(ClaimDefinition requiredClaim) : this([requiredClaim]) { }

    };
}


/// <summary>
/// Version of <see cref="InternalPolicyDefinition"/> strongly associated with a standard
/// </summary>
/// <typeparam name="TStandard"></typeparam>
/// <param name="Name"></param>
/// <param name="Description"></param>
/// <param name="StrongerThanAdmin"></param>
/// <param name="AppInternalClaimRequirementAlternatives"></param>
public record InternalPolicyDefinition<TStandard>(
    string Name,
    string Description,
    bool StrongerThanAdmin,
    IReadOnlyCollection<InternalPolicyDefinition<TStandard>.ClaimRequirement> AppInternalClaimRequirementAlternatives)
    : InternalPolicyDefinition(
        Name,
        Description,
        StrongerThanAdmin,
        AppInternalClaimRequirementAlternatives.Select(x => x.ToBaseRequirement()).ToArray())
    where TStandard : IInternalAuthStandard
{
    public record ClaimRequirement(IReadOnlyCollection<AppInternalClaimDefinition<TStandard>> RequiredClaims)
    {
        public ClaimRequirement(AppInternalClaimDefinition<TStandard> requiredClaim) : this([requiredClaim]) { }

        public InternalPolicyDefinition.ClaimRequirement ToBaseRequirement()
            => new InternalPolicyDefinition.ClaimRequirement(RequiredClaims);

    };
}