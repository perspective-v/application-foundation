using PerspectiveV.Common.Extensions;
using System.Diagnostics;
using System.Reflection;

namespace PerspectiveV.Common.Helpers;
/// <summary>
/// Represents the CsToTsConverter type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of CsToTsConverter.
/// var type = typeof(CsToTsConverter);
/// ]]></code>
/// </example>
public class CsToTsConverter {
    /// <summary>
    /// Executes Convert Class.
    /// </summary>
    /// <param name="type">The type value.</param>
    /// <param name="includeConstants">The includeConstants value.</param>
    /// <returns>The operation result.</returns>
    public static string ConvertClass(Type type, bool includeConstants = false) {

        string tsClass = $"export class {type.Name}";
        tsClass += " {\n";
        type.GetProperties().ToList().ForEach(property => {
            tsClass += $"    public {property.Name.ToCamelCase()}{isNullable(property)}: {GetTypeName(property)};\n";
        });
        if (includeConstants) {
            var constProps = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
            if (constProps.Count != 0) {
                constProps.ForEach(property => {
                    tsClass += $"    public static {property.Name} = '{property.GetValue(type)}';\n";

                });
            }
        }
        tsClass += "}";

        Console.WriteLine(tsClass);
        Debug.WriteLine(tsClass);
        return tsClass;
    }

    /// <summary>
    /// Executes Convert Classes.
    /// </summary>
    /// <param name="types">The types value.</param>
    /// <returns>The operation result.</returns>
    public static string ConvertClasses(params Type[] types) {
        string classes = "";
        foreach (var type in types) {
            // check if the class extends/inherite some other class
            if (type is not null && type.BaseType is not null && !type.BaseType.Name.Equals(nameof(Object))) {
                string tsClass = $"export class {type.Name} extends {type.BaseType.Name}";
                var baseClassPropertyNames = type.BaseType.GetProperties().Select(property => property.Name).ToList();
                tsClass += " {\n";
                type.GetProperties().Where(property => !baseClassPropertyNames.Contains(property.Name)).ToList().ForEach(property => {
                    tsClass += $"    public {property.Name.ToCamelCase()}{isNullable(property)}: {GetTypeName(property)};\n";
                });
                tsClass += "}\n";
                Console.WriteLine(tsClass);
                classes += tsClass;
            }
            else {
                string tsClass = $"export class {type?.Name}";
                tsClass += " {\n";
                type?.GetProperties()?.ToList()?.ForEach(property => {
                    tsClass += $"    public {property.Name.ToCamelCase()}{isNullable(property)}: {GetTypeName(property)};\n";
                });
                tsClass += "}\n";

                Console.WriteLine(tsClass);
                classes += tsClass;
            }
        }
        return classes;
    }

    static string GetTypeName(PropertyInfo property) {
        var res = "";
        var permitives = new List<Type>() {
            typeof(string),
            typeof(bool),
            typeof(Nullable<bool>),
            typeof(DateTime),
            typeof(Nullable<DateTime>),
            typeof(int),
            typeof(decimal),
            typeof(double),
            typeof(long),
            typeof(float),
            typeof(Nullable<int>),
            typeof(Nullable<decimal>),
            typeof(Nullable<double>),
            typeof(Nullable<long>),
            typeof(Nullable<float>),
            typeof(List<>),
            typeof(ICollection<>),
            typeof(IEnumerable<>),
            typeof(IList<>),
        };
        if (permitives.Contains(property.PropertyType)) {
            res = CheckPermitiveType(property.PropertyType);

        }
        else if (property.PropertyType.IsGenericType && (property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))) {
            Type listItemType = property.PropertyType.GetGenericArguments().First();
            if (listItemType != null) {
                res = $"{CheckPermitiveType(listItemType)}[]";
            }
            else {
                res = "any[]";
            }
        }
        //else if(property.PropertyType.IsGenericType && (property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))) {
        //    res = "any[]";
        //} 
        else {
            res = $"{property.PropertyType.Name}";
        }
        return res;
    }

    static string CheckPermitiveType(Type type) {
        var res = type.Name;
        if (type == typeof(string)) {
            res = "string";
        }
        if (type == typeof(bool) ||
            type == typeof(bool?) ||
            type == typeof(Nullable<bool>)) {
            res = "boolean";
        }

        if (type == typeof(DateTime) || type == typeof(Nullable<DateTime>)) {
            res = "Date";
        }

        if (type == typeof(int) ||
            type == typeof(decimal) ||
            type == typeof(double) ||
            type == typeof(long) ||
            type == typeof(float) ||
            type == typeof(Nullable<int>) ||
            type == typeof(Nullable<decimal>) ||
            type == typeof(Nullable<double>) ||
            type == typeof(Nullable<long>) ||
            type == typeof(Nullable<float>)) {
            res = "number";
        }
        return res;
    }
    static string isNullable(PropertyInfo property) {
        var res = "";
        var permitives = new List<Type>() {
            typeof(Nullable<bool>),
            typeof(Nullable<DateTime>),
            typeof(Nullable<int>),
            typeof(Nullable<decimal>),
            typeof(Nullable<double>),
            typeof(Nullable<long>),
            typeof(Nullable<float>),
            typeof(List<>),
            typeof(ICollection<>),
            typeof(IEnumerable<>),
            typeof(IList<>),
        };
        if (permitives.Contains(property.PropertyType)) {
            res = "?";

        }
        else if (property.PropertyType.IsGenericType && (property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))) {
            res = "?";
        }
        return res;
    }
}
