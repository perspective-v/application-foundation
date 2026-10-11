namespace PerspectiveV.EntityFramework.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PerspectiveV.Primitives.Types.Interfaces;
using PerspectiveV.Primitives.Types.Models;
using System.Linq.Expressions;

public static class EfProperty
{
    public static Expression Property(
        Expression entity,
        string propertyName,
        Type propertyType)
    {
        return Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            new[] { propertyType },
            entity,
            Expression.Constant(propertyName)
        );
    }
}

public static class EfCoreExtensions
{
    private static Type GetIdType<EntityType>()
    {
        if (typeof(IEntity<Guid>).IsAssignableFrom(typeof(EntityType)))
            return typeof(Guid);

        if (typeof(IEntity<int>).IsAssignableFrom(typeof(EntityType)))
            return typeof(int);

        if (typeof(IEntity<long>).IsAssignableFrom(typeof(EntityType)))
            return typeof(long);

        if (typeof(IEntity<string>).IsAssignableFrom(typeof(EntityType)))
            return typeof(string);

        throw new NotSupportedException($"Unsupported Id type for {typeof(EntityType)}");
    }


    public static Expression<Func<TEntity, bool>> ByIdDynamic<TEntity>(object id)
    {
        var p = Expression.Parameter(typeof(TEntity), "x");

        var idType = GetIdType<TEntity>();

        var idExpr = EfProperty.Property(
            p,
            "Id",
            idType
        );

        var body = Expression.Equal(
            idExpr,
            Expression.Constant(id, idType)
        );

        return Expression.Lambda<Func<TEntity, bool>>(body, p);
    }

    // Build a dynamic filter for Id (generic)
    public static Expression<Func<TEntity, bool>> ByTId<TEntity, TId>(TId id)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var idProperty = Expression.Property(parameter, nameof(IEntity.Id));
        var idValue = Expression.Constant(id, typeof(TId));
        var body = Expression.Equal(idProperty, idValue);
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    public static Expression<Func<TEntity, bool>> IsDeleted<TEntity>(bool getSoftDeleted)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var deletedProperty = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));

        Expression body;

        if (deletedProperty.Type == typeof(bool?))
        {
            // nullable bool: treat null as false
            body = Expression.Equal(
                Expression.Coalesce(deletedProperty, Expression.Constant(false)),
                Expression.Constant(getSoftDeleted)
            );
        }
        else
        {
            body = Expression.Equal(deletedProperty, Expression.Constant(getSoftDeleted));
        }

        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    /// <summary>
    /// Builds an expression to filter entities implementing IAppClient by AppClientId.
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="clientId">The client ID string to match</param>
    /// <returns>Expression usable in EF Core Where()</returns>
    public static Expression<Func<TEntity, bool>> HasClientId<TEntity>(string clientId)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var property = Expression.Property(parameter, nameof(IAppClient.AppClientId));

        Expression body;

        if (property.Type == typeof(bool?))
        {
            // nullable bool: treat null as false
            body = Expression.Equal(
                Expression.Coalesce(property, Expression.Constant(false)),
                Expression.Constant(clientId)
            );
        }
        else
        {
            body = Expression.Equal(property, Expression.Constant(clientId));
        }

        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    public static Expression<Func<TEntity, bool>> NotDeleted<TEntity>()
        where TEntity : ISoftDeletable
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var deletedProperty = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
        var body = Expression.IsFalse(deletedProperty);
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    public static Expression<Func<TEntity, NameValuePair<TId>>> ToNameValuePair<TEntity, TId>()
    where TEntity : class, INamedEntity<TId>
    {
        return x => new NameValuePair<TId>
        {
            Value = x.Id,
            Name = x.Name ?? "No Name"
        };
    }

    //public static Expression<Func<TEntity, NameValuePair<TID>>> ToNameValuePair<TEntity,TID>()
    //where TEntity : class
    //{
    //    var param = Expression.Parameter(typeof(TEntity), "x");

    //    // cast to INamedEntity<Guid>
    //    var cast = Expression.TypeAs(param, typeof(INamedEntity<TID>));

    //    var idProp = Expression.Property(cast, nameof(INamedEntity<TID>.Id));
    //    var nameProp = Expression.Property(cast, nameof(INamedEntity<TID>.Name));

    //    // coalesce for null name
    //    var coalesce = Expression.Coalesce(nameProp, Expression.Constant("No Name"));

    //    var body = Expression.MemberInit(
    //        Expression.New(typeof(NameValuePair<TID>)),
    //        Expression.Bind(typeof(NameValuePair<TID>).GetProperty(nameof(NameValuePair<TID>.Value)), idProp),
    //        Expression.Bind(typeof(NameValuePair<TID>).GetProperty(nameof(NameValuePair<TID>.Name)), coalesce)
    //    );

    //    return Expression.Lambda<Func<TEntity, NameValuePair<TID>>>(body, param);
    //}

    //public static Expression<Func<TEntity, NameValuePair<TId>>> ToNameValuePair<TEntity, TId>(
    //Expression<Func<TEntity, TId>> idSelector,
    //Expression<Func<TEntity, string>>? nameSelector = null)
    //where TEntity : class, INamedEntity<TId>
    //{
    //    var param = idSelector.Parameters[0];

    //    Expression nameExpr;
    //    if (nameSelector != null)
    //    {
    //        // replace parameter
    //        var replacer = new ParameterReplacer(nameSelector.Parameters[0], param);
    //        var body = replacer.Visit(nameSelector.Body)!;
    //        nameExpr = Expression.Coalesce(body, Expression.Constant("No Name"));
    //    }
    //    else
    //    {
    //        nameExpr = Expression.Constant("No Name");
    //    }

    //    var bodyInit = Expression.MemberInit(
    //        Expression.New(typeof(NameValuePair<TId>)),
    //        Expression.Bind(typeof(NameValuePair<TId>).GetProperty(nameof(NameValuePair<TId>.Value)), idSelector.Body),
    //        Expression.Bind(typeof(NameValuePair<TId>).GetProperty(nameof(NameValuePair<TId>.Name)), nameExpr)
    //    );

    //    return Expression.Lambda<Func<TEntity, NameValuePair<TId>>>(bodyInit, param);
    //}

    //// helper for parameter replacement
    //private class ParameterReplacer : ExpressionVisitor
    //{
    //    private readonly ParameterExpression _old;
    //    private readonly ParameterExpression _new;
    //    public ParameterReplacer(ParameterExpression oldParam, ParameterExpression newParam)
    //    {
    //        _old = oldParam;
    //        _new = newParam;
    //    }

    //    protected override Expression VisitParameter(ParameterExpression node)
    //        => node == _old ? _new : base.VisitParameter(node);
    //}
}
