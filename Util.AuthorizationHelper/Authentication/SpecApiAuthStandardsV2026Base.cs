namespace Util.AuthorizationHelper.Authentication;

/// <summary>
/// Base class for all v2026 standards
/// </summary>
public abstract class SpecApiAuthStandardsV2026Base 
{
    public const string MyDefaultHeaderName = "X-Api-Key";
    public const string MyDefaultSchemeName = "ApiKey";
    public static string DefaultHeaderName => MyDefaultHeaderName;
    public static string DefaultSchemeName => MyDefaultSchemeName;
}