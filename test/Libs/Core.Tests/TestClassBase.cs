using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Seedysoft.Libs.Core.Tests;

public abstract class TestClassBase : Libs.Tests.TUnitTestClassBase
{
    protected static Mock<IServiceProvider> MockServiceProvider { get; private set; } = default!;
    protected static Mock<IHostApplicationLifetime> MockHostApplicationLifetime { get; private set; } = default!;
    protected static Mock<IConfiguration> MockConfiguration { get; private set; } = default!;

    protected CancellationTokenSource CancellationTokenSource { get; private set; } = default!;

    //public static IServiceCollection AddDbContext(IServiceCollection services)
    //{
    //    Microsoft.Data.Sqlite.SqliteConnection connection = new("Filename=:memory:");
    //    connection.Open();

    //    using (DbContexts.DbCtx context = new(new DbContextOptionsBuilder<DbContexts.DbCtx>().UseSqlite(connection).Options))
    //        _ = context.Database.EnsureCreated();

    //    _ = services
    //        .AddDbContext<DbContexts.DbCtx>((dbContextOptionsBuilder) =>
    //        {
    //            _ = dbContextOptionsBuilder
    //                .UseSqlite(connection)
    //                .ConfigureDebugOptions();
    //        }
    //        , ServiceLifetime.Transient
    //        , ServiceLifetime.Transient);

    //    return services;
    //}

    [Before(Class)]
    public static void SetupClass()
    {
        MockServiceProvider = new Mock<IServiceProvider>();
        MockHostApplicationLifetime = new Mock<IHostApplicationLifetime>();
        MockConfiguration = new Mock<IConfiguration>();

        // Setup service provider to return required services
        _ = MockServiceProvider
            .Setup(static x => x.GetService(typeof(IConfiguration)))
            .Returns(MockConfiguration.Object);
    }

    [After(Class)]
    public static void CleanupClass()
    {

    }

    [Before(Test)]
    public virtual void SetupTest() => CancellationTokenSource = new CancellationTokenSource();

    [After(Test)]
    public virtual void CleanupTest() => CancellationTokenSource?.Dispose();
}
