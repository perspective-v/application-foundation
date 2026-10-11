using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Models; 
/// <summary>
/// Represents the SoftDeletableModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SoftDeletableModel.
/// var type = typeof(SoftDeletableModel);
/// ]]></code>
/// </example>
public class SoftDeletableModel : Model, ISoftDeletable {
    public bool IsDeleted { get; set; }
}
/// <summary>
/// Represents the SoftDeletableModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SoftDeletableModel.
/// var type = typeof(SoftDeletableModel<>);
/// ]]></code>
/// </example>
public class SoftDeletableModel<IdType> : Model<IdType>, ISoftDeletable {
    public bool IsDeleted { get; set; }
}
