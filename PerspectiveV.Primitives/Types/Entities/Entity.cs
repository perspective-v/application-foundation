using PerspectiveV.Primitives.Types.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PerspectiveV.Primitives.Types.Entities; 
/// <summary>
/// Represents the Entity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of Entity.
/// var type = typeof(Entity);
/// ]]></code>
/// </example>
public class Entity : IEntity {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
}

/// <summary>
/// Represents the Entity type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of Entity.
/// var type = typeof(Entity<>);
/// ]]></code>
/// </example>
public class Entity<IdType> : IEntity<IdType>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public IdType Id { get; set; }
}
