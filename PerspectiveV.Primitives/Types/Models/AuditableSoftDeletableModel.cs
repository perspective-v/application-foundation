using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Models; 
/// <summary>
/// Represents the AuditableSoftDeletableModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableSoftDeletableModel.
/// var type = typeof(AuditableSoftDeletableModel);
/// ]]></code>
/// </example>
public class AuditableSoftDeletableModel : SoftDeletableModel, IAuditable {
    public string? CreatedBy { get; set; }
    public Nullable<DateTime> CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public Nullable<DateTime> UpdatedDate { get; set; }
}

/// <summary>
/// Represents the AuditableSoftDeletableModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableSoftDeletableModel.
/// var type = typeof(AuditableSoftDeletableModel<>);
/// ]]></code>
/// </example>
public class AuditableSoftDeletableModel<IdType> : SoftDeletableModel<IdType>, IAuditable {
    public string? CreatedBy { get; set; }
    public Nullable<DateTime> CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public Nullable<DateTime> UpdatedDate { get; set; }
}
