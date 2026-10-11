using PerspectiveV.Primitives.Types.Models;
using System.ComponentModel;
using System.Reflection;

namespace PerspectiveV.Common.Extensions;
/// <summary>
/// Provides static members in Enum Extension.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by EnumExtension.
/// var typeName = nameof(EnumExtension);
/// ]]></code>
/// </example>
public static class EnumExtension {
    public static List<NameValuePair<TPairType>> GetNameValue<TPairType, TEnum>() where TEnum : Enum {
        var list = new List<NameValuePair<TPairType>>(Enum.GetValues(typeof(TEnum)).Length);

        foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum))) {
            var name = enumValue.ToString();
            var description = typeof(TEnum)
                .GetField(name)
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description ?? name;

            list.Add(new NameValuePair<TPairType> {
                Name = description,
                Value = (TPairType)(object)(int)(object)enumValue
            });
        }

        return list;
    }


    public static string GetDescription(this Enum value) {
        var type = value.GetType();
        var name = Enum.GetName(type, value);

        if (string.IsNullOrEmpty(name))
            return string.Empty;

        var field = type.GetField(name);
        if (field == null)
            return name;

        var attr = field.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? name;
    }
}
