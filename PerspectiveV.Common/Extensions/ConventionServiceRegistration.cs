using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PerspectiveV.Common.Extensions;

/// <summary>
/// Provides static members in Convention Service Registration.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by ConventionServiceRegistration.
/// var typeName = nameof(ConventionServiceRegistration);
/// ]]></code>
/// </example>
public static class ConventionServiceRegistration
{
    /// <summary>
    /// Adds Add Conventions.
    /// </summary>
    /// <param name="services">The services value.</param>
    /// <param name="namespaceContains">The namespaceContains value.</param>
    /// <param name="assemblies">The assemblies value.</param>
    /// <returns>The operation result.</returns>
    public static IServiceCollection AddConventions(
        this IServiceCollection services,
        string namespaceContains,
        params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
            return services;

        var types = assemblies
            .Where(a => a != null)
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return Array.Empty<Type>(); }
            })
            .ToArray();

        var map = new Dictionary<Type, Type>();

        var interfaces = types.Where(t =>
            t.IsInterface &&
            !string.IsNullOrWhiteSpace(t.Namespace) &&
            t.Namespace.Contains(namespaceContains, StringComparison.OrdinalIgnoreCase));

        foreach (var iface in interfaces)
        {
            var impl = types.FirstOrDefault(c =>
                c.IsClass &&
                !c.IsAbstract &&
                !string.IsNullOrWhiteSpace(c.Namespace) &&
                c.Namespace.Contains(namespaceContains, StringComparison.OrdinalIgnoreCase) &&
                iface.IsAssignableFrom(c) &&
                c.Name.Equals(iface.Name.Substring(1), StringComparison.OrdinalIgnoreCase));

            if (impl != null && !map.ContainsKey(iface))
            {
                map[iface] = impl;
            }
        }

        foreach (var (service, implementation) in map)
        {
            services.AddScoped(service, implementation);
        }

        return services;
    }
}
