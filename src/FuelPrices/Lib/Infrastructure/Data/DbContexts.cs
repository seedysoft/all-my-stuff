using Microsoft.EntityFrameworkCore;
using Seedysoft.FuelPrices.Lib.Core.Entities;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Data;

public abstract class CarburantesDbContextBase : DbContext
{
#if DEBUG
    public CarburantesDbContextBase() : base() { }
#endif
    public CarburantesDbContextBase(DbContextOptions options) : base(options) => ChangeTracker.LazyLoadingEnabled = false;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
    }
}

public sealed class CarburantesDbContext : CarburantesDbContextBase
{
#if DEBUG
    public CarburantesDbContext() : base() { }
#endif

#if !DEBUG
#pragma warning disable IDE0290 // Use primary constructor
#endif
    public CarburantesDbContext(DbContextOptions<CarburantesDbContext> options) : base(options) { }
#if !DEBUG
#pragma warning restore IDE0290 // Use primary constructor
#endif

#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string DatabasePath = "../../../../../../../databases/sqlite3";
            string FullFilePath = Path.GetFullPath(DatabasePath);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            string ConnectionString = $"{Libs.Core.Constants.DatabaseStrings.DataSource}{FullFilePath}";
            Console.WriteLine(ConnectionString);

            _ = optionsBuilder.UseSqlite(ConnectionString);
        }
    }
#endif

    protected override void OnModelCreating(ModelBuilder modelBuilder) => base.OnModelCreating(modelBuilder);

    public DbSet<ComunidadAutonoma> ComunidadesAutonomas { get; set; } = default!;
    public DbSet<EstacionProductoPrecio> EstacionProductoPrecios { get; set; } = default!;
    public DbSet<EstacionServicio> EstacionesServicio { get; set; } = default!;
    public DbSet<Municipio> Municipios { get; set; } = default!;
    public DbSet<ProductoPetrolifero> ProductosPetroliferos { get; set; } = default!;
    public DbSet<Provincia> Provincias { get; set; } = default!;

    public DbSet<ProductPrice> ProductPriceView { get; set; } = default!;
}

public sealed class CarburantesHistDbContext : CarburantesDbContextBase
{
#if DEBUG
    public CarburantesHistDbContext() : base() { }
#endif

#if !DEBUG
#pragma warning disable IDE0290 // Use primary constructor
#endif
    public CarburantesHistDbContext(DbContextOptions<CarburantesHistDbContext> options) : base(options) { }
#if !DEBUG
#pragma warning restore IDE0290 // Use primary constructor
#endif

#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string DatabasePath = "../../../../../../../databases/CarburantesHist.sqlite3";
            string FullFilePath = Path.GetFullPath(DatabasePath);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            string ConnectionString = $"{Libs.Core.Constants.DatabaseStrings.DataSource}{FullFilePath}";
            Console.WriteLine(ConnectionString);

            _ = optionsBuilder.UseSqlite(ConnectionString);
        }
    }
#endif

    protected override void OnModelCreating(ModelBuilder modelBuilder) => base.OnModelCreating(modelBuilder);

    public DbSet<ComunidadAutonomaHist> ComunidadesAutonomasHist { get; set; } = default!;
    public DbSet<EstacionProductoPrecioHist> EstacionProductoPreciosHist { get; set; } = default!;
    public DbSet<EstacionServicioHist> EstacionesServicioHist { get; set; } = default!;
    public DbSet<MunicipioHist> MunicipiosHist { get; set; } = default!;
    public DbSet<ProductoPetroliferoHist> ProductosPetroliferosHist { get; set; } = default!;
    public DbSet<ProvinciaHist> ProvinciasHist { get; set; } = default!;

    public DbSet<ProductPriceHist> ProductPriceHistView { get; set; } = default!;
}
