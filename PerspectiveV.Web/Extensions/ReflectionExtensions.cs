using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace PerspectiveV.Web.Extensions;

/// <summary>
/// Provides extension methods for Reflection Extensions.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by ReflectionExtensions.
/// var typeName = nameof(ReflectionExtensions);
/// ]]></code>
/// </example>
public static class ReflectionExtensions {

    /// <summary>
    /// Gets Get Attribute.
    /// </summary>
    /// <param name="member">The member value.</param>
    /// <param name="isRequired">The isRequired value.</param>
    /// <returns>The operation result.</returns>
    public static T? GetAttribute<T>(this MemberInfo member, bool isRequired = false) where T : Attribute {
        var attribute = member.GetCustomAttribute<T>();

        if (attribute == null && isRequired) {
            throw new ArgumentException(
                $"The {typeof(T).Name} attribute must be defined on member {member.Name}",
                nameof(member));
        }

        return attribute;
    }

    /// <summary>
    /// Gets Get Property Display Name.
    /// </summary>
    /// <param name="propertyExpression">The propertyExpression value.</param>
    /// <returns>The operation result.</returns>
    public static string GetPropertyDisplayName<T>(Expression<Func<T, object>> propertyExpression) {
        var memberInfo = GetPropertyInformation(propertyExpression.Body)
                         ?? throw new ArgumentException("No property reference expression was found.", nameof(propertyExpression));

        return memberInfo.GetAttribute<DisplayNameAttribute>()?.DisplayName ?? memberInfo.Name;
    }

    /// <summary>
    /// Gets Get Property Information.
    /// </summary>
    /// <param name="propertyExpression">The propertyExpression value.</param>
    /// <returns>The operation result.</returns>
    public static MemberInfo? GetPropertyInformation(Expression? propertyExpression) {
        if (propertyExpression is null) {
            return null;
        }

        if (propertyExpression is MemberExpression memberExpr && memberExpr.Member.MemberType == MemberTypes.Property) {
            return memberExpr.Member;
        }

        if (propertyExpression is UnaryExpression unaryExpr && unaryExpr.NodeType == ExpressionType.Convert) {
            var memberExprFromUnary = unaryExpr.Operand as MemberExpression;
            if (memberExprFromUnary?.Member.MemberType == MemberTypes.Property) {
                return memberExprFromUnary.Member;
            }
        }

        return null;
    }

}
