namespace PerspectiveV.Primitives.Types.Interfaces; 
public interface INamedModel : IModel {
    public string? Name { get; set; }
}
public interface INamedModel<IdType> : IModel<IdType> {
    public string? Name { get; set; }
}
