using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Models; 
/// <summary>
/// Represents the NamedModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of NamedModel.
/// var type = typeof(NamedModel);
/// ]]></code>
/// </example>
public class NamedModel : Model, INamedModel {
    public string? Name { get; set; }

}
/// <summary>
/// Represents the NamedModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of NamedModel.
/// var type = typeof(NamedModel<>);
/// ]]></code>
/// </example>
public class NamedModel<IdType> : Model<IdType>, INamedModel<IdType> {
    public string? Name { get; set; }
}
