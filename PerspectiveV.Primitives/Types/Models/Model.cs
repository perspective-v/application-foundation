using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Models; 
/// <summary>
/// Represents the Model type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of Model.
/// var type = typeof(Model);
/// ]]></code>
/// </example>
public class Model : IModel {
    public Guid? Id { get; set; }
}

/// <summary>
/// Represents the Model type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of Model.
/// var type = typeof(Model<>);
/// ]]></code>
/// </example>
public class Model<IdType> : IModel<IdType> {
    public IdType? Id { get; set; }
}
