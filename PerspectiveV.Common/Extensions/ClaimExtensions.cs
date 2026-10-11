using PerspectiveV.Common.Identity;
using System.Security.Claims;
using System.Security.Principal;

namespace PerspectiveV.Common.Extensions;
/// <summary>
/// Provides extension methods for Claim Extensions.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by ClaimExtensions.
/// var typeName = nameof(ClaimExtensions);
/// ]]></code>
/// </example>
public static class ClaimExtensions {
    /// <summary>
    /// Gets Get User Id.
    /// </summary>
    /// <param name="identity">The identity value.</param>
    /// <returns>The operation result.</returns>
    public static string? GetUserId(this IIdentity identity) {
        if (identity == null) return null;

        var id = (identity as ClaimsIdentity)?.FirstOrNull(ClaimTypes.NameIdentifier);
        return id;
    }

    /// <summary>
    /// Determines whether Has Roles.
    /// </summary>
    /// <param name="identity">The identity value.</param>
    /// <param name="roles">The roles value.</param>
    /// <returns>The operation result.</returns>
    public static bool HasRoles(this IIdentity identity, params string[] roles) {
        bool hasMatch = false;
        if (identity == null)
            return hasMatch;

        var claimRoles = (identity as ClaimsIdentity)?.FindAll(ClaimTypes.Role).ToList();
        if (claimRoles?.Count > 0) {
            hasMatch = claimRoles.Any(ci => roles.Contains(ci.Value));
        }
        return hasMatch;
    }

    /// <summary>
    /// Determines whether Has Claims.
    /// </summary>
    /// <param name="identity">The identity value.</param>
    /// <param name="claims">The claims value.</param>
    /// <returns>The operation result.</returns>
    public static bool HasClaims(this IIdentity identity, params string[] claims) {
        bool hasMatch = false;

        if (identity == null) return hasMatch;

        var ids = (identity as ClaimsIdentity)?.FindAll(AccountClaimTypes.CLAIMS).ToList();
        if (ids?.Count > 0) {
            hasMatch = ids.Any(ci => claims.Contains(ci.Value));
        }

        return hasMatch;
    }

    /// <summary>
    /// Determines whether Has Roles String.
    /// </summary>
    /// <param name="identity">The identity value.</param>
    /// <param name="roles">The roles value.</param>
    /// <returns>The operation result.</returns>
    public static bool HasRolesString(this IIdentity identity, params string[] roles) {
        if (identity == null)
            return false;

        var roleNames = (identity as ClaimsIdentity)?.FirstOrNull(AccountClaimTypes.ROLES);
        return (roleNames != null && roleNames.Length > 0 && roleNames.Contains(string.Join(",", roles)) == true) ? true : false;
    }

    /// <summary>
    /// Determines whether Has Claims String.
    /// </summary>
    /// <param name="identity">The identity value.</param>
    /// <param name="claims">The claims value.</param>
    /// <returns>The operation result.</returns>
    public static bool HasClaimsString(this IIdentity identity, params string[] claims) {
        if (identity == null) return false;

        var ids = (identity as ClaimsIdentity)?.FirstOrNull(AccountClaimTypes.CLAIMS);
        return (ids != null && ids.Length > 0 && ids.Contains(string.Join(",", claims)) == true) ? true : false;
    }

    /// <summary>
    /// Gets Get App Client Id.
    /// </summary>
    /// <param name="identity">The identity value.</param>
    /// <returns>The operation result.</returns>
    public static string? GetAppClientId(this IIdentity identity) {
        if (identity == null) return null;

        var clientId = (identity as ClaimsIdentity)?.FirstOrNull(AccountClaimTypes.APP_CLIENT_IDENTIFIER);
        return clientId;

    }

    /// <summary>
    /// Gets Get Role Ids.
    /// </summary>
    /// <param name="identity">The identity value.</param>
    /// <returns>The operation result.</returns>
    public static string? GetRoleIds(this ClaimsIdentity identity) {
        if (identity == null) return null;

        var clientId = (identity).FirstOrNull(AccountClaimTypes.ROLE_IDENTIFIERS);
        return clientId;
    }

    /// <summary>
    /// Executes First Or Null.
    /// </summary>
    /// <param name="identity">The identity value.</param>
    /// <param name="claimType">The claimType value.</param>
    /// <returns>The operation result.</returns>
    internal static string? FirstOrNull(this ClaimsIdentity identity, string claimType) {
        var val = identity.FindFirst(claimType);

        return val == null ? null : val.Value;
    }

    /// <summary>
    /// Determines whether Has Permission.
    /// </summary>
    /// <param name="User">The User value.</param>
    /// <param name="module">The module value.</param>
    /// <param name="action">The action value.</param>
    /// <returns>The operation result.</returns>
    public static bool HasPermission(this ClaimsPrincipal User, string module, string action) {
        return User.HasClaim(AccountClaimConsts.TYPE_PERMISSION, string.Join(".", module.ToLower(), action.ToLower()));
    }

    /// <summary>
    /// Determines whether Has Permission.
    /// </summary>
    /// <param name="User">The User value.</param>
    /// <param name="claim">The claim value.</param>
    /// <returns>The operation result.</returns>
    public static bool HasPermission(this ClaimsPrincipal User, string claim) {
        return User.HasClaim(AccountClaimConsts.TYPE_PERMISSION, claim);
    }

    /// <summary>
    /// Determines whether Has Permissions.
    /// </summary>
    /// <param name="User">The User value.</param>
    /// <param name="claims">The claims value.</param>
    /// <returns>The operation result.</returns>
    public static bool HasPermissions(this ClaimsPrincipal User, string[] claims) {
        var hasClaim = false;
        foreach (var claim in claims) {
            if (User.HasClaim(AccountClaimConsts.TYPE_PERMISSION, claim)) {
                hasClaim = true;
            }
        }
        return hasClaim;
    }

    /// <summary>
    /// Gets Get App Code.
    /// </summary>
    /// <param name="identity">The identity value.</param>
    /// <returns>The operation result.</returns>
    public static string? GetAppCode(this IIdentity identity) {
        return (identity as ClaimsIdentity)?.FirstOrNull(AccountClaimTypes.APP_CODE);
    }

    /// <summary>
    /// Gets Get App Subscription.
    /// </summary>
    /// <param name="identity">The identity value.</param>
    /// <returns>The operation result.</returns>
    public static string? GetAppSubscription(this IIdentity identity) {
        return (identity as ClaimsIdentity)?.FirstOrNull(AccountClaimTypes.APP_CLIENT_SUBSCRIPTION_IDENTIFIERS);
    }

    /// <summary>
    /// Determines whether Can Create.
    /// </summary>
    /// <param name="User">The User value.</param>
    /// <param name="module">The module value.</param>
    /// <returns>The operation result.</returns>
    public static bool CanCreate(this ClaimsPrincipal User, string module) => User.HasPermission(module, AccountClaimConsts.PERMISSION_CREATE);
    /// <summary>
    /// Determines whether Can Update.
    /// </summary>
    /// <param name="User">The User value.</param>
    /// <param name="module">The module value.</param>
    /// <returns>The operation result.</returns>
    public static bool CanUpdate(this ClaimsPrincipal User, string module) => User.HasPermission(module, AccountClaimConsts.PERMISSION_EDIT);
    /// <summary>
    /// Determines whether Can Delete.
    /// </summary>
    /// <param name="User">The User value.</param>
    /// <param name="module">The module value.</param>
    /// <returns>The operation result.</returns>
    public static bool CanDelete(this ClaimsPrincipal User, string module) => User.HasPermission(module, AccountClaimConsts.PERMISSION_DELETE);
    /// <summary>
    /// Determines whether Can View.
    /// </summary>
    /// <param name="User">The User value.</param>
    /// <param name="module">The module value.</param>
    /// <returns>The operation result.</returns>
    public static bool CanView(this ClaimsPrincipal User, string module) => User.HasPermission(module, AccountClaimConsts.PERMISSION_VIEW);

    /// <summary>
    /// Gets Get Pfp Url.
    /// </summary>
    /// <param name="User">The User value.</param>
    /// <returns>The operation result.</returns>
    public static string GetPfpUrl(this ClaimsPrincipal User) {
        string purl = User.Claims.SingleOrDefault(c => c.Type == AccountClaimTypes.PROFILE_PIC_URL)?.Value ?? string.Empty;

        if (string.IsNullOrEmpty(purl))
            return "";
        else
            return purl;

    }
}
