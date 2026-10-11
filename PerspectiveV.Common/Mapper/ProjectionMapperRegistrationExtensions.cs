namespace PerspectiveV.Common.Mapper;

using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;


/// <summary>
/// Registers Mapperly mapper classes as
/// <see cref="IProjectionMapper{TEntity, TModel}"/> services.
/// <para>
/// The scanner validates a mapper class and wires it into DI so consumers can depend on
/// <see cref="IProjectionMapper{TEntity, TModel}"/> instead of concrete mapper classes.
/// </para>
/// <para>
/// Mandatory mapper members expected by this registration:
/// <code><![CDATA[
/// [Mapper]
/// public partial class UserMapper
/// {
///     // Mandatory object mapping methods
///     public partial UserModel ToModel(UserEntity entity);
///     public partial UserEntity ToEntity(UserModel model);
///
///     // Mandatory projection expression methods
///     public static partial Expression<Func<UserEntity, UserModel>> ToModelExpression();
///     public static partial Expression<Func<UserModel, UserEntity>> ToEntityExpression();
/// }
/// ]]></code>
/// </para>
/// <para>
/// Usage example:
/// <code><![CDATA[
/// builder.Services.AddMapperlyMappers(typeof(UserMapper).Assembly);
///
/// // Resolve wrapper contract anywhere in the app:
/// // IProjectionMapper<UserEntity, UserModel>
/// ]]></code>
/// </para>
/// <para>
/// Wrapper-free direct usage (no IProjectionMapper adapter):
/// <code><![CDATA[
/// services.AddScoped<UserMapper>();
///
/// var model = mapper.ToModel(entity);
/// var query = dbContext.Users.Select(UserMapper.ToModelExpression());
/// ]]></code>
/// </para>
/// <para>
/// Explanation: without the wrapper, your app consumes the Mapperly class directly and must call
/// its mapping methods/expressions explicitly.
/// </para>
/// </summary>
public static class ProjectionMapperRegistrationExtensions
{
    /// <summary>
    /// Scans the provided assemblies for mapper classes and registers projection mappers in DI.
    /// </summary>
    /// <param name="services">Service collection used by the application.</param>
    /// <param name="assemblies">Assemblies to scan for classes decorated with Mapperly's mapper attribute.</param>
    /// <returns>The same service collection for fluent chaining.</returns>
    public static IServiceCollection AddMapperlyMappers(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var mapperTypes = assemblies
            .SelectMany(SafeGetTypes)
            .Where(IsMapper)
            .ToList();

        foreach (var mapperType in mapperTypes)
        {
            RegisterMapperOrThrow(services, mapperType);
        }

        return services;
    }

    // =========================================================
    // INTERNAL HELPERS
    // =========================================================

    /// <summary>
    /// Safely returns all loadable types from an assembly.
    /// </summary>
    /// <param name="assembly">Assembly to inspect.</param>
    /// <returns>All discovered types, or an empty set if reflection fails.</returns>
    private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try { return assembly.GetTypes(); }
        catch { return []; }
    }

    /// <summary>
    /// Determines whether a type qualifies as a mapper implementation.
    /// </summary>
    /// <param name="type">Candidate type to evaluate.</param>
    /// <returns><see langword="true"/> when the type is a non-abstract class with a mapper attribute.</returns>
    private static bool IsMapper(Type type)
    {
        return type.IsClass &&
               !type.IsAbstract &&
               type.GetCustomAttributes()
                   .Any(a => a.GetType().Name == "MapperAttribute");
    }

    /// <summary>
    /// Validates mapper method conventions and registers the mapper pair.
    /// </summary>
    /// <param name="services">Service collection receiving the registration.</param>
    /// <param name="mapperType">Concrete mapper type discovered during assembly scanning.</param>
    private static void RegisterMapperOrThrow(IServiceCollection services, Type mapperType)
    {
        var methods = mapperType.GetMethods();

        // STEP 1: Find entity-to-model mapping.
        var toModel = methods.FirstOrDefault(m =>
            m.Name.Equals("ToModel", StringComparison.Ordinal) &&
            m.ReturnType != typeof(void) &&
            m.GetParameters().Length == 1 &&
            !typeof(IQueryable).IsAssignableFrom(m.ReturnType) &&
            !typeof(Expression).IsAssignableFrom(m.ReturnType));

        if (toModel == null)
            Throw(mapperType, "Missing mandatory mapping method: ToModel(EntityType)");

        var idType = toModel.GetParameters()[0].ParameterType;
        var entityType = toModel.GetParameters()[1].ParameterType;
        var modelType = toModel.ReturnType;

        // STEP 2: Find model-to-entity mapping.
        var toEntity = methods.FirstOrDefault(m =>
            m.Name.Equals("ToEntity", StringComparison.Ordinal) &&
            m.ReturnType == entityType &&
            m.GetParameters().Length == 1 &&
            m.GetParameters()[0].ParameterType == modelType);

        if (toEntity == null)
            Throw(mapperType, $"Missing mandatory reverse mapping method: ToEntity({modelType.Name}) -> {entityType.Name}");

        // STEP 3: Find entity-to-model expression.
        var toModelExpression = methods.FirstOrDefault(m =>
            m.Name.Equals("ToModelExpression", StringComparison.Ordinal) &&
            IsExpressionMapping(m, entityType, modelType));

        if (toModelExpression == null)
            Throw(mapperType, $"Missing mandatory projection expression: ToModelExpression() for {entityType.Name} -> {modelType.Name}");

        // STEP 4: Find model-to-entity expression.
        var toEntityExpression = methods.FirstOrDefault(m =>
            m.Name.Equals("ToEntityExpression", StringComparison.Ordinal) &&
            IsExpressionMapping(m, modelType, entityType));

        if (toEntityExpression == null)
            Throw(mapperType, $"Missing mandatory reverse projection expression: ToEntityExpression() for {modelType.Name} -> {entityType.Name}");

        // STEP 5: Find model-to-entity expression.
        var toNameValueExpression = methods.FirstOrDefault(m =>
            m.Name.Equals("ToNameValueExpression", StringComparison.Ordinal) &&
            IsExpressionMapping(m, modelType, entityType));

        
        // STEP 6: Register.
        RegisterProjectionMapper(
            services,
            mapperType,
            idType,
            entityType,
            modelType,
            toModel,
            toEntity!,
            toModelExpression!,
            toEntityExpression!,
            toNameValueExpression!
            );
    }

    /// <summary>
    /// Checks whether a method returns <see cref="Expression{TDelegate}"/> for the specified type pair.
    /// </summary>
    /// <param name="method">Method candidate from the mapper class.</param>
    /// <param name="from">Expected source type inside the expression delegate.</param>
    /// <param name="to">Expected destination type inside the expression delegate.</param>
    /// <returns><see langword="true"/> when the signature matches an expression mapping for the provided pair.</returns>
    private static bool IsExpressionMapping(MethodInfo method, Type from, Type to)
    {
        if (!method.ReturnType.IsGenericType) return false;

        if (method.ReturnType.GetGenericTypeDefinition() != typeof(Expression<>))
            return false;

        var lambdaType = method.ReturnType.GenericTypeArguments[0];

        if (!lambdaType.IsGenericType) return false;

        var args = lambdaType.GenericTypeArguments;

        return args.Length == 2 &&
               args[0] == from &&
               args[1] == to;
    }

    /// <summary>
    /// Creates and registers a scoped projection mapper adapter for a mapper type.
    /// </summary>
    /// <param name="services">Service collection receiving the scoped registration.</param>
    /// <param name="mapperType">Mapper class that provides mapping methods.</param>
    /// <param name="entityType">Entity type.</param>
    /// <param name="modelType">Model type.</param>
    /// <param name="toModel">Forward mapping method (entity to model).</param>
    /// <param name="toEntity">Reverse mapping method (model to entity).</param>
    /// <param name="toModelExpression">Forward projection expression method.</param>
    /// <param name="toEntityExpression">Reverse projection expression method.</param>
    private static void RegisterProjectionMapper(
        IServiceCollection services,
        Type mapperType,
        Type idType,
        Type entityType,
        Type modelType,
        MethodInfo toModel,
        MethodInfo toEntity,
        MethodInfo toModelExpression,
        MethodInfo toEntityExpression,
        MethodInfo toNameValueExpression
        )
    {
        var interfaceType = typeof(IProjectionMapper<,,>)
            .MakeGenericType(idType, entityType, modelType);

        var implementationType = typeof(ProjectionMapper<,,>)
            .MakeGenericType(idType, entityType, modelType);

        services.AddScoped(interfaceType, provider =>
        {
            var mapperInstance = Activator.CreateInstance(mapperType)!;

            var toModelDelegate = toModel.CreateDelegate(
                typeof(Func<,>).MakeGenericType(idType, entityType, modelType),
                mapperInstance);

            var toEntityDelegate = toEntity.CreateDelegate(
                typeof(Func<,>).MakeGenericType(idType, modelType, entityType),
                mapperInstance);

            var toModelExpr = (Expression)toModelExpression.Invoke(mapperInstance, null)!;
            var toEntityExpr = (Expression)toEntityExpression.Invoke(mapperInstance, null)!;
            var toNameValueExpr = (Expression)toNameValueExpression.Invoke(mapperInstance, null)!;

            return Activator.CreateInstance(
                implementationType,
                toModelDelegate,
                toEntityDelegate,
                toModelExpr,
                toEntityExpr,
                toNameValueExpr)!;
        });
    }

    // =========================================================
    // ERROR HANDLING
    // =========================================================

    /// <summary>
    /// Throws a detailed exception when a mapper does not satisfy the required conventions.
    /// </summary>
    /// <param name="mapperType">Mapper type that failed validation.</param>
    /// <param name="message">Specific validation failure message.</param>
    [DoesNotReturn]
    private static void Throw(Type mapperType, string message)
    {
        throw new InvalidOperationException(
            $"Invalid mapper '{mapperType.Name}': {message}\n" +
            $"Ensure it defines mandatory members:\n" +
            $"- ToModel(EntityType)\n" +
            $"- ToEntity(ModelType)\n" +
            $"- ToModelExpression() returning Expression<Func<EntityType, ModelType>>\n" +
            $"- ToEntityExpression() returning Expression<Func<ModelType, EntityType>>");
    }
}
