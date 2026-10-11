using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Entities; 
/// <summary>
/// Represents the AuditableEntity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableEntity.
/// var type = typeof(AuditableEntity);
/// ]]></code>
/// </example>
public class AuditableEntity : Entity, IAuditable {
    public string? CreatedBy { get; set; }
    public Nullable<DateTime> CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public Nullable<DateTime> UpdatedDate { get; set; }
}

/// <summary>
/// Represents the AuditableEntity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableEntity.
/// var type = typeof(AuditableEntity<>);
/// ]]></code>
/// </example>
public class AuditableEntity<IdType> : Entity<IdType>, IAuditable {
    public string? CreatedBy { get; set; }
    public Nullable<DateTime> CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public Nullable<DateTime> UpdatedDate { get; set; }
}
