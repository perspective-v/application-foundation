using PerspectiveV.Common.Extensions;

namespace PerspectiveV.Common.Identity;
/// <summary>
/// Represents configuration options for Module Claim Options.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of ModuleClaimOptions.
/// var type = typeof(ModuleClaimOptions);
/// ]]></code>
/// </example>
public class ModuleClaimOptions {
    public string? ModuleName { get; set; }
    public string? ServiceName { get; set; }
    public string? AppCode { get; set; }
    public string[]? Permissions { get; set; }
    public bool UseDefaultPermissions { get; set; } = false;
}

/// <summary>
/// Represents the ModuleClaimModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of ModuleClaimModel.
/// var type = typeof(ModuleClaimModel);
/// ]]></code>
/// </example>
public class ModuleClaimModel {
    public List<string> DefaultPermissions { get; set; } = new List<string>(){
           "view",
           "create",
           "edit",
           "delete",
    };

    /// <summary>
    /// Initializes a new instance of the ModuleClaimModel class.
    /// </summary>
    /// <param name="moduleName">The moduleName value.</param>
    /// <param name="permissions">The permissions value.</param>
    public ModuleClaimModel(string moduleName, params string[] permissions) {
        _moduleName = moduleName;
        ModuleName = moduleName;
        ClaimsList = new List<PermissionClaimModel>();
        if (permissions != null && permissions.Length > 0) {
            foreach (var permission in permissions) {
                ClaimsList.Add(new PermissionClaimModel {
                    Name = $"{permission.Replace("_", " ").Replace('-', ' ').ToSentenceCase()}",
                    Value = $"{ModuleNameValue}.{permission.ToLower()}"
                });
            }
        }
    }
    /// <summary>
    /// Initializes a new instance of the ModuleClaimModel class.
    /// </summary>
    /// <param name="options">The options value.</param>
    public ModuleClaimModel(ModuleClaimOptions options) {
        if (options == null || string.IsNullOrEmpty(options.ModuleName)) throw new ArgumentNullException(nameof(options), $"{nameof(ModuleClaimOptions)} options can not be null");

        _moduleName = options.ModuleName;
        ModuleName = _moduleName;
        AppCode = options.AppCode;
        ServiceName = options.ServiceName;
        ClaimsList = new List<PermissionClaimModel>();
        var mPermissions = new List<string>();
        if (options?.UseDefaultPermissions == true) {
            mPermissions.AddRange(DefaultPermissions);

            if (options.Permissions != null && options.Permissions.Length > 0) {
                mPermissions.AddRange(options.Permissions);
            }

            options.Permissions = mPermissions.ToArray();
        }

        if (options?.Permissions != null && options?.Permissions.Length > 0) {
            foreach (var permission in options.Permissions) {
                ClaimsList.Add(new PermissionClaimModel {
                    Name = $"{permission.Replace("_", " ").Replace('-', ' ').ToSentenceCase()}",
                    Value = $"{ModuleNameValue}.{permission.ToLower()}"
                });
            }
        }
    }


    /// <summary>
    /// Under consideration don't make public
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="addConditionResult"></param>
    /// <param name="permissions"></param>
    //private ModuleClaimModel(string moduleName, bool addConditionResult = true, params string[] permissions) {
    //    ModuleName = moduleName;
    //    if (permissions != null && permissions.Length > 0) {
    //        foreach (var permission in permissions) {
    //            ClaimsList?.Add(new PermissionClaimModel {
    //                Name = $"{permission.Replace("_", " ").Replace('-', ' ').ToUpper().ToSentenceCase()}",
    //                Value = $"{ModuleNameValue}.{permission.ToLower()}"
    //            });
    //        }
    //    }
    //}

    private string _moduleName;
    public string ModuleName {
        get { return _moduleName.Replace("_", " ").Replace('-', ' ').ToSentenceCase(); }
        set { _moduleName = value; }
    }

    public string ModuleNameValue {
        get { return ModuleName.Replace(" ", "").Replace("_", ".").Replace('-', '.').ToLower(); }
    }

    public string? ServiceName { get; set; }
    public string? AppCode { get; set; }

    public string AppName
    {
        get { return (AppCode ?? string.Empty).Replace("_", " ").Replace('-', ' ').ToSentenceCase(); }
    }

    public List<PermissionClaimModel> ClaimsList { get; set; }

    public string[] Permissions {
        get {
            var list = new List<string>();
            if (ClaimsList != null && ClaimsList.Count != 0) {
                foreach (var claim in ClaimsList) {
                    if (claim is not null && claim.Value is not null) {
                        list.Add(claim.Value.Split(".").Last());
                    }
                }
            }
            return [.. list];
        }
    }
}
