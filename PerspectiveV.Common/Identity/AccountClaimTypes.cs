namespace PerspectiveV.Common.Identity;
/// <summary>
/// Represents the AccountClaimTypes type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AccountClaimTypes.
/// var type = typeof(AccountClaimTypes);
/// ]]></code>
/// </example>
public partial class AccountClaimTypes {
    public const string USER_IDENTIFIER = "userid";
    public const string USER_NAME = "username";
    public const string PROFILE_PIC_URL = "profilepicurl";
    public const string ROLE = "role";
    public const string ROLES = "roles";
    public const string ROLE_NAMES = "rolenames";
    public const string ROLE_IDENTIFIERS = "roleids";

    public const string CLAIMS = "claims";
    public const string PERMISSION = "permission";

    public const string APP_CODE = "appcode";
    public const string APP_CLIENT_IDENTIFIER = "appclientid";
    public const string APP_CLIENT_SUBSCRIPTION_IDENTIFIERS = "appclientsubscriptionids";
}
