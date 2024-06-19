using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.UtilsLib.Dependencies;
using System.Reflection;

namespace Seedysoft.UtilsLib.Extensions;

public static class IHostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddAllMyDependencies(this IHostApplicationBuilder hostApplicationBuilder)
    {
        Assembly entryAssembly = Assembly.GetEntryAssembly()!;

        if (System.Diagnostics.Debugger.IsAttached)
        {
            _ = hostApplicationBuilder.Logging.AddConsole();
            _ = hostApplicationBuilder.Configuration.SetBasePath(Path.GetDirectoryName(entryAssembly.Location)!);
        }

        _ = hostApplicationBuilder.Configuration
            .AddEnvironmentVariables();

        IEnumerable<Assembly> referencedAssemblies = GetAllReferencedAssemblies(entryAssembly);

        IEnumerable<Type> typesToRegister = referencedAssemblies.SelectMany((Assembly n) => n.GetTypes())
            .Where((Type t) => t is not null && t.IsSubclassOf(typeof(ConfiguratorBase)))
            .ToArray();

        foreach (Type? type in typesToRegister)
            (Activator.CreateInstance(type) as ConfiguratorBase)?.AddDependencies(hostApplicationBuilder);

        return hostApplicationBuilder;
    }

    public static IEnumerable<Assembly> GetAllReferencedAssemblies(Assembly source)
    {
        var results = new List<Assembly> { source };

        results.AddRange(source.GetReferencedAssemblies().SelectMany((AssemblyName name) =>
        {
            var loaded = Assembly.Load(name);
            return loaded.GetTypes().Any((Type t) => t is not null && t.IsSubclassOf(typeof(ConfiguratorBase)))
                ? GetAllReferencedAssemblies(loaded)
                : Array.Empty<Assembly>();
        }).Distinct());

        return results.Distinct();
    }

    //public static WebApplication MigrateDbContexts(this WebApplication webApplication)
    //{
    //    webApplication.Services.GetRequiredService<InfrastructureLib.DbContexts.DbCxt>().Database.Migrate();
    //    webApplication.Services.GetRequiredService<Seedysoft.CarburantesLib.Services.ObtainDataCronBackgroundService>().MigrateDatabases();

    //    return webApplication;
    //}
}
