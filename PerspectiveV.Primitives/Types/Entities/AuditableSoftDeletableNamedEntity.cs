using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Entities; 
/// <summary>
/// Represents the AuditableSoftDeletableNamedEntity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableSoftDeletableNamedEntity.
/// var type = typeof(AuditableSoftDeletableNamedEntity);
/// ]]></code>
/// </example>
public class AuditableSoftDeletableNamedEntity : AuditableSoftDeletableEntity, INamedEntity {
    public string? Name { get; set; }
}

/// <summary>
/// Represents the AuditableSoftDeletableNamedEntity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableSoftDeletableNamedEntity.
/// var type = typeof(AuditableSoftDeletableNamedEntity<>);
/// ]]></code>
/// </example>
public class AuditableSoftDeletableNamedEntity<IdType> : AuditableSoftDeletableEntity<IdType>, INamedEntity<IdType>
{
    public string? Name { get; set; }
}
