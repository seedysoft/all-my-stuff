namespace Seedysoft.Libs.Infrastructure.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    public static Microsoft.EntityFrameworkCore.DbContextOptionsBuilder ConfigureDebugOptions(this Microsoft.EntityFrameworkCore.DbContextOptionsBuilder dbContextOptionsBuilder)
    {
        if (System.Diagnostics.Debugger.IsAttached)
        {
            dbContextOptionsBuilder = dbContextOptionsBuilder
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Debug);
        }

        return dbContextOptionsBuilder;
    }
}
