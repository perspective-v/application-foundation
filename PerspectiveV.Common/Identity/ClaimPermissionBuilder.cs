using System.Reflection;

namespace PerspectiveV.Common.Identity;
/// <summary>
/// Represents the ClaimPermissionBuilder type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of ClaimPermissionBuilder.
/// var type = typeof(ClaimPermissionBuilder);
/// ]]></code>
/// </example>
public class ClaimPermissionBuilder {
    private static List<string> claimsList = new List<string>();

    public static List<string> _permissions { get; set; } = new List<string>(){
           "view",
           "create",
           "edit",
           "delete",
    };

    public static List<ModuleClaimModel> _modules { get; set; } = new List<ModuleClaimModel>();

    /// <summary>
    /// Gets Get Claims List.
    /// </summary>
    /// <returns>The operation result.</returns>
    public static List<string> GetClaimsList() {
        _modules.ForEach(module => {
            var claims = module.ClaimsList?.Select(x => x.Value).Where(x => x != null)!.ToList();
            if (claims != null && claims.Count != 0) {
                claimsList.AddRange(claims);
            }
        });
        return claimsList;
    }

    /// <summary>
    /// Sets Set Module Claims.
    /// </summary>
    /// <param name="moduleClaims">The moduleClaims value.</param>
    public static void SetModuleClaims(params ModuleClaimModel[] moduleClaims) {
        _modules = [.. moduleClaims];
    }

    //public static void SetDefaultPermissions(params string[] permissions) {
    //    _permissions = permissions.ToList();
    //}
    //public static List<string> GetDefaultPermissions(params string[] permissions) {
    //    return _permissions;
    //}

    /// <summary>
    /// Gets Get Module Claims List.
    /// </summary>
    /// <returns>The operation result.</returns>
    public static List<ModuleClaimModel> GetModuleClaimsList() {
        return _modules;
    }

    /// <summary>
    /// Executes Generate Or Update Claim Keys.
    /// </summary>
    /// <param name="appNamespace">The appNamespace value.</param>
    /// <param name="claimFileName">The claimFileName value.</param>
    public static void GenerateOrUpdateClaimKeys(string appNamespace, string claimFileName = "AccountClaimKeys") {
        try {
            if (string.IsNullOrEmpty(appNamespace)) {
                appNamespace = $"{Assembly.GetExecutingAssembly().GetName().Name}";
                appNamespace = $"{Assembly.GetCallingAssembly().GetName().Name}";
            }
            #region Generate file code
            var fileName = $"{claimFileName}.cs";
            var callingProjectDir = new DirectoryInfo(Environment.CurrentDirectory);
            var solutionDir = new DirectoryInfo(callingProjectDir?.Parent?.FullName!).GetDirectories();
            var commonProjDirs = new DirectoryInfo(solutionDir.Where(dir => dir.Name.Equals(appNamespace, StringComparison.OrdinalIgnoreCase)).Single().FullName).GetDirectories();
            var identity = new DirectoryInfo(commonProjDirs.Where(x => x.Name.Equals("identity", StringComparison.OrdinalIgnoreCase)).Single().FullName);
            var claimsFile = identity.GetFiles().Where(cf => cf.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            #region ClaimkeysGeneration Script
            var contentStr = $@"namespace {appNamespace}.{identity.Name};" + $"\n\npublic class AccountClaimKeys {{" + $"\n";
            _modules.ForEach(module => {
                contentStr += $"\t#region {module.ModuleName.ToUpper()}\n";
                if (module.ClaimsList != null && module.ClaimsList.Count != 0) {
                    module.ClaimsList?.ForEach(permission => {
                        var claimSymbol = $"{module.ModuleNameValue?.Trim().Replace(" ", "_").ToUpper()}_{permission?.Name?.Trim().Replace(" ", "_").ToUpper()}";
                        var constField = $"\t" + $@" public const string {claimSymbol} = ""{permission?.Value}"";" + "\n";
                        contentStr += constField;
                    });
                }
                contentStr += $"\t#endregion\n\n";
            });
            contentStr += "}";
            #endregion

            if (claimsFile != null && claimsFile.Exists == true) {
                File.WriteAllText(claimsFile.FullName, contentStr);
            }
            else {
                using (var sr = File.CreateText(Path.Combine(identity.FullName, fileName))) {
                    sr.Write(contentStr);
                    sr.Close();
                }
            }
            #endregion
        }
        catch (Exception) {
            throw;
        }
    }
}
