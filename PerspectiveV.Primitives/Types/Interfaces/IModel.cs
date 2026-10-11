namespace PerspectiveV.Primitives.Types.Interfaces; 
public interface IModel : IModel<Guid?> {

}

public interface IModel<T> {
    public T? Id { get; set; }
}
