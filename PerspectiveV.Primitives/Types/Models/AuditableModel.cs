using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Models; 
/// <summary>
/// Represents the AuditableModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableModel.
/// var type = typeof(AuditableModel);
/// ]]></code>
/// </example>
public class AuditableModel : Model, IAuditable {
    public string? CreatedBy { get; set; }
    public Nullable<DateTime> CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public Nullable<DateTime> UpdatedDate { get; set; }
}

/// <summary>
/// Represents the AuditableModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AuditableModel.
/// var type = typeof(AuditableModel<>);
/// ]]></code>
/// </example>
public class AuditableModel<IdType> : Model<IdType>, IAuditable {
    public string? CreatedBy { get; set; }
    public Nullable<DateTime> CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public Nullable<DateTime> UpdatedDate { get; set; }
}
