using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Entities; 
/// <summary>
/// Represents the AuditableSoftDeletableEntity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableSoftDeletableEntity.
/// var type = typeof(AuditableSoftDeletableEntity);
/// ]]></code>
/// </example>
public class AuditableSoftDeletableEntity : SoftDeletableEntity, IAuditable {
    public string? CreatedBy { get; set; }
    public Nullable<DateTime> CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public Nullable<DateTime> UpdatedDate { get; set; }
}

/// <summary>
/// Represents the AuditableSoftDeletableEntity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableSoftDeletableEntity.
/// var type = typeof(AuditableSoftDeletableEntity<>);
/// ]]></code>
/// </example>
public class AuditableSoftDeletableEntity<IdType> : SoftDeletableEntity<IdType>, IAuditable {
    public string? CreatedBy { get; set; }
    public Nullable<DateTime> CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public Nullable<DateTime> UpdatedDate { get; set; }
}
