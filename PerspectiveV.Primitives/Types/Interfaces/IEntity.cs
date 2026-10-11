namespace PerspectiveV.Primitives.Types.Interfaces; 
public interface IEntity : IEntity<Guid> {

}

public interface IEntity<T> {
    public T Id { get; set; }
}
