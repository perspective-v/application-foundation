namespace PerspectiveV.Primitives.Types.Interfaces; 
public interface INamedEntity : IEntity {
    public string? Name { get; set; }
}
public interface INamedEntity<IdType> : IEntity<IdType>
{
    public string? Name { get; set; }
}