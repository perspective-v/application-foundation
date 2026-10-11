namespace PerspectiveV.Common.Identity; 
/// <summary>
/// Provides static members in Account Claim Consts.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by AccountClaimConsts.
/// var typeName = nameof(AccountClaimConsts);
/// ]]></code>
/// </example>
public static class AccountClaimConsts
{
    public const string SEPARATOR = ".";
    public const string TYPE_PERMISSION = "permission";
    public const string PERMISSION_VIEW = "view";
    public const string PERMISSION_CREATE = "create";
    public const string PERMISSION_EDIT = "edit";
    public const string PERMISSION_DELETE = "delete";
}
