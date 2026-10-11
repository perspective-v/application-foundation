using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Entities; 
/// <summary>
/// Represents the SoftDeletableNamedEntity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SoftDeletableNamedEntity.
/// var type = typeof(SoftDeletableNamedEntity);
/// ]]></code>
/// </example>
public class SoftDeletableNamedEntity : NamedEntity, ISoftDeletable {
    public bool IsDeleted { get; set; }
}

/// <summary>
/// Represents the SoftDeletableNamedEntity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SoftDeletableNamedEntity.
/// var type = typeof(SoftDeletableNamedEntity<>);
/// ]]></code>
/// </example>
public class SoftDeletableNamedEntity<IdType> : NamedEntity<IdType>, ISoftDeletable {
    public bool IsDeleted { get; set; }
}
