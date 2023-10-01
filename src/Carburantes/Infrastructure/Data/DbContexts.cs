using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Seedysoft.Carburantes.Infrastructure.Data;

public abstract class CarburantesDbContextBase : DbContext
{
    public CarburantesDbContextBase(DbContextOptions options) : base(options) => ChangeTracker.LazyLoadingEnabled = false;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
    }
}

public sealed class CarburantesDbContext : CarburantesDbContextBase
{
    public CarburantesDbContext(DbContextOptions<CarburantesDbContext> options) : base(options) => ChangeTracker.LazyLoadingEnabled = false;

    protected override void OnModelCreating(ModelBuilder modelBuilder) => base.OnModelCreating(modelBuilder);

    public DbSet<Core.Entities.ComunidadAutonoma> ComunidadesAutonomas { get; set; } = default!;
    public DbSet<Core.Entities.EstacionProductoPrecio> EstacionProductoPrecios { get; set; } = default!;
    public DbSet<Core.Entities.EstacionServicio> EstacionesServicio { get; set; } = default!;
    public DbSet<Core.Entities.Municipio> Municipios { get; set; } = default!;
    public DbSet<Core.Entities.ProductoPetrolifero> ProductosPetroliferos { get; set; } = default!;
    public DbSet<Core.Entities.Provincia> Provincias { get; set; } = default!;
}

public sealed class CarburantesHistDbContext : CarburantesDbContextBase
{
    public CarburantesHistDbContext(DbContextOptions<CarburantesHistDbContext> options) : base(options) => ChangeTracker.LazyLoadingEnabled = false;

    protected override void OnModelCreating(ModelBuilder modelBuilder) => base.OnModelCreating(modelBuilder);

    public DbSet<Core.Entities.ComunidadAutonomaHist> ComunidadesAutonomasHist { get; set; } = default!;
    public DbSet<Core.Entities.EstacionProductoPrecioHist> EstacionProductoPreciosHist { get; set; } = default!;
    public DbSet<Core.Entities.EstacionServicioHist> EstacionesServicioHist { get; set; } = default!;
    public DbSet<Core.Entities.MunicipioHist> MunicipiosHist { get; set; } = default!;
    public DbSet<Core.Entities.ProductoPetroliferoHist> ProductosPetroliferosHist { get; set; } = default!;
    public DbSet<Core.Entities.ProvinciaHist> ProvinciasHist { get; set; } = default!;
}

#if DEBUG
/// <summary>
/// Con esta clase podemos generar las migraciones
/// </summary>
public class CarburantesDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<CarburantesDbContext>
{
    public CarburantesDbContextFactory(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public CarburantesDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<CarburantesDbContext> builder = new();

        const string ConnectionStringName = nameof(CarburantesDbContext);
        string ConnectionString = Configuration.GetConnectionString($"{ConnectionStringName}")?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
        string FullFilePath = Path.GetFullPath(ConnectionString["Data Source=".Length..]);
        if (!File.Exists(FullFilePath))
            throw new FileNotFoundException("Database file not found: '{FullPath}'", FullFilePath);

        _ = builder.UseSqlite(ConnectionString);

        return new CarburantesDbContext(builder.Options);
    }
}

/// <summary>
/// Con esta clase podemos generar las migraciones
/// </summary>
public class CarburantesHistDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<CarburantesHistDbContext>
{
    public CarburantesHistDbContextFactory(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public CarburantesHistDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<CarburantesHistDbContext> builder = new();

        const string ConnectionStringName = nameof(CarburantesHistDbContext);
        string ConnectionString = Configuration.GetConnectionString($"{ConnectionStringName}")?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
        string FullFilePath = Path.GetFullPath(ConnectionString["Data Source=".Length..]);
        if (!File.Exists(FullFilePath))
            throw new FileNotFoundException("Database file not found: '{FullPath}'", FullFilePath);

        _ = builder.UseSqlite(ConnectionString);

        return new CarburantesHistDbContext(builder.Options);
    }
}
#endif
