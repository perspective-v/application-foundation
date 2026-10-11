using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Models; 
/// <summary>
/// Represents the AuditableSoftDeletableNamedModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableSoftDeletableNamedModel.
/// var type = typeof(AuditableSoftDeletableNamedModel);
/// ]]></code>
/// </example>
public class AuditableSoftDeletableNamedModel : AuditableSoftDeletableModel, INamedModel {
    public string? Name { get; set; }
}
/// <summary>
/// Represents the AuditableSoftDeletableNamedModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableSoftDeletableNamedModel.
/// var type = typeof(AuditableSoftDeletableNamedModel<>);
/// ]]></code>
/// </example>
public class AuditableSoftDeletableNamedModel<IdType> : AuditableSoftDeletableModel<IdType>, INamedModel<IdType>
{
    public string? Name { get; set; }
}
