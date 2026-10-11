using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Models;
/// <summary>
/// Represents the SoftDeletableNamedModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SoftDeletableNamedModel.
/// var type = typeof(SoftDeletableNamedModel);
/// ]]></code>
/// </example>
public class SoftDeletableNamedModel : NamedModel, ISoftDeletable
{
    public bool IsDeleted { get; set; }
}
/// <summary>
/// Represents the SoftDeletableNamedModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SoftDeletableNamedModel.
/// var type = typeof(SoftDeletableNamedModel<>);
/// ]]></code>
/// </example>
public class SoftDeletableNamedModel<IdType> : NamedModel<IdType>, ISoftDeletable
{
    public bool IsDeleted { get; set; }
}
