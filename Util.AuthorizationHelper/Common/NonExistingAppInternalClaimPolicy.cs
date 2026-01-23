namespace Util.AuthorizationHelper.Common;

/// <summary>
/// What should some context do when a non-registered / non-existent claim is encountered
/// </summary>
public enum NonExistingAppInternalClaimPolicy
{
    /// <summary>
    /// Should be considered invalid / not set
    /// </summary>
    Null,
    /// <summary>
    /// Issue a message but don't abort operation
    /// </summary>
    Warn,
    /// <summary>
    /// Stop the operation (e.g. by throwing an exception)
    /// </summary>
    Abort,
    /// <summary>
    /// Pretend it didn't happen
    /// </summary>
    Ignore
}