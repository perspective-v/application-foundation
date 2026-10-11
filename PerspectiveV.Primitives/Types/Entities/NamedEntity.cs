using PerspectiveV.Primitives.Types.Interfaces;

namespace PerspectiveV.Primitives.Types.Entities; 
/// <summary>
/// Represents the NamedEntity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of NamedEntity.
/// var type = typeof(NamedEntity);
/// ]]></code>
/// </example>
public class NamedEntity : Entity, INamedEntity {
    public string? Name { get; set; }
}

/// <summary>
/// Represents the NamedEntity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of NamedEntity.
/// var type = typeof(NamedEntity<>);
/// ]]></code>
/// </example>
public class NamedEntity<IdType> : Entity<IdType>, INamedEntity<IdType>
{
    public string? Name { get; set; }
}
