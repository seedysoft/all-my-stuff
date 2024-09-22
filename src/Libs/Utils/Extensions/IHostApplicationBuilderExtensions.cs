using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Seedysoft.Libs.Utils.Extensions;

public static class IHostApplicationBuilderExtensions
{
    public static Microsoft.Extensions.Hosting.IHostApplicationBuilder AddAllMyDependencies(
        this Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetCallingAssembly()!;

        if (System.Diagnostics.Debugger.IsAttached)
        {
            _ = hostApplicationBuilder.Logging.AddConsole();
            _ = hostApplicationBuilder.Configuration.SetBasePath(Path.GetDirectoryName(entryAssembly.Location)!);
        }

        _ = hostApplicationBuilder.Configuration
            .AddEnvironmentVariables();

        IEnumerable<System.Reflection.Assembly> referencedAssemblies = GetAllReferencedAssembliesSorted(entryAssembly);

        Type[] typesToRegister = referencedAssemblies.SelectMany(static (System.Reflection.Assembly n) => n.GetTypes())
            .Where(static (Type t) => t is not null && t.IsSubclassOf(typeof(Dependencies.ConfiguratorBase)))
            .ToArray();

        foreach (Type? type in typesToRegister)
            (Activator.CreateInstance(type) as Dependencies.ConfiguratorBase)?.AddDependencies(hostApplicationBuilder);

        return hostApplicationBuilder;
    }

    public static IEnumerable<System.Reflection.Assembly> GetAllReferencedAssembliesSorted(System.Reflection.Assembly source)
    {
        var results = new List<System.Reflection.Assembly> { source };

        results.InsertRange(0, source.GetReferencedAssemblies().SelectMany(static (System.Reflection.AssemblyName name) =>
        {
            var loaded = System.Reflection.Assembly.Load(name);

            return loaded.DefinedTypes.Any(static (Type t) => t is not null && t.IsSubclassOf(typeof(Dependencies.ConfiguratorBase)))
                ? GetAllReferencedAssembliesSorted(loaded)
                : [];
        }).Distinct());

        return results.Distinct();
    }
}
