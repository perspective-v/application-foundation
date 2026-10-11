using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Entities; 
/// <summary>
/// Represents the SoftDeletableEntity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SoftDeletableEntity.
/// var type = typeof(SoftDeletableEntity);
/// ]]></code>
/// </example>
public class SoftDeletableEntity : Entity, ISoftDeletable {
    public bool IsDeleted { get; set; }
}

/// <summary>
/// Represents the SoftDeletableEntity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SoftDeletableEntity.
/// var type = typeof(SoftDeletableEntity<>);
/// ]]></code>
/// </example>
public class SoftDeletableEntity<IdType> : Entity<IdType>, ISoftDeletable
{
    public bool IsDeleted { get; set; }
}

