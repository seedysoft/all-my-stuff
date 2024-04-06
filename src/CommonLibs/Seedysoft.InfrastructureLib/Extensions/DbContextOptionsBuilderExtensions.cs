namespace Seedysoft.InfrastructureLib.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    public static void ConfigureDebugOptions(this Microsoft.EntityFrameworkCore.DbContextOptionsBuilder dbContextOptionsBuilder)
    {
        if (System.Diagnostics.Debugger.IsAttached)
        {
            _ = dbContextOptionsBuilder
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Trace);
        }
    }
}
