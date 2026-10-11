using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Entities; 
/// <summary>
/// Represents the AuditableNamedEntity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableNamedEntity.
/// var type = typeof(AuditableNamedEntity);
/// ]]></code>
/// </example>
public class AuditableNamedEntity : AuditableEntity, INamedEntity {
    public string? Name { get; set; }
}
/// <summary>
/// Represents the AuditableNamedEntity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableNamedEntity.
/// var type = typeof(AuditableNamedEntity<>);
/// ]]></code>
/// </example>
public class AuditableNamedEntity<IdType> : AuditableEntity<IdType>, INamedEntity<IdType> {
    public string? Name { get; set; }
}
