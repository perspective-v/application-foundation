namespace PerspectiveV.Common; 
/// <summary>
/// Represents the AppConstants type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AppConstants.
/// var type = typeof(AppConstants);
/// ]]></code>
/// </example>
public class AppConstants {
    public const string SITE_INFO_COOKIE = "sinf";
    /// <summary>
    /// Represents the AuthorizationErrors type.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// // Create and use an instance of AuthorizationErrors.
    /// var type = typeof(AuthorizationErrors);
    /// ]]></code>
    /// </example>
    public sealed class AuthorizationErrors
    {
        public const string VIEW = "Unauthorized: You can not view this resource.";
        public const string CREATE = "Unauthorized: You can not create this resource.";
        public const string EDIT = "Unauthorized: You can not update this resource.";
        public const string DELETE = "Unauthorized: You can not delete this resource.";
    } 
    
    /// <summary>
    /// Represents the AuthenticationErrors type.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// // Create and use an instance of AuthenticationErrors.
    /// var type = typeof(AuthenticationErrors);
    /// ]]></code>
    /// </example>
    public sealed class AuthenticationErrors
    {
        public const string INVALID_USERNAME_OR_PASSWORD = "Invalid Username Or Password";
    }

    /// <summary>
    /// Represents the Messages type.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// // Create and use an instance of Messages.
    /// var type = typeof(Messages);
    /// ]]></code>
    /// </example>
    public sealed class Messages {
        public const string NULL_PARAMETERS = "Invalid Username Or Password";
        public const string VERIFICATION_CODE_EXPIRED = "Verification code has expired.";
        public const string INVALID_USERNAME= "Invalid Username";
        public const string INVALID_PASSWORD = "Invalid Password";

    }

    #region Media Folders
    /// <summary>
    /// Represents the FolderPaths type.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// // Create and use an instance of FolderPaths.
    /// var type = typeof(FolderPaths);
    /// ]]></code>
    /// </example>
    public sealed class FolderPaths {
        public const string MEDIA = "~/media";
    }
    #endregion

}

