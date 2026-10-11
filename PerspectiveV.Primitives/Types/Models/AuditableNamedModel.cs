using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Models; 
/// <summary>
/// Represents the AuditableNamedModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableNamedModel.
/// var type = typeof(AuditableNamedModel);
/// ]]></code>
/// </example>
public class AuditableNamedModel : AuditableModel, INamedModel {
    public string? Name { get; set; }
}
/// <summary>
/// Represents the AuditableNamedModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableNamedModel.
/// var type = typeof(AuditableNamedModel<>);
/// ]]></code>
/// </example>
public class AuditableNamedModel<IdType> : AuditableModel<IdType>, INamedModel<IdType>  {
    public string? Name { get; set; }
}
