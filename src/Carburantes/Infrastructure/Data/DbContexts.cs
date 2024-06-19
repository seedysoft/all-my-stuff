using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Carburantes.Infrastructure.Data;

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
            string DatabasePath = "../../../../../../../databases/Carburantes.sqlite3";
            string FullFilePath = Path.GetFullPath(DatabasePath);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            string ConnectionString = $"{CoreLib.Constants.DatabaseStrings.DataSource}{FullFilePath}";
            Console.WriteLine(ConnectionString);

            _ = optionsBuilder.UseSqlite(ConnectionString);
        }
    }
#endif

    protected override void OnModelCreating(ModelBuilder modelBuilder) => base.OnModelCreating(modelBuilder);

    public DbSet<Core.Entities.ComunidadAutonoma> ComunidadesAutonomas { get; set; } = default!;
    public DbSet<Core.Entities.EstacionProductoPrecio> EstacionProductoPrecios { get; set; } = default!;
    public DbSet<Core.Entities.EstacionServicio> EstacionesServicio { get; set; } = default!;
    public DbSet<Core.Entities.Municipio> Municipios { get; set; } = default!;
    public DbSet<Core.Entities.ProductoPetrolifero> ProductosPetroliferos { get; set; } = default!;
    public DbSet<Core.Entities.Provincia> Provincias { get; set; } = default!;

    public DbSet<Core.Entities.ProductPrice> ProductPriceView { get; set; } = default!;
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

            string ConnectionString = $"{CoreLib.Constants.DatabaseStrings.DataSource}{FullFilePath}";
            Console.WriteLine(ConnectionString);

            _ = optionsBuilder.UseSqlite(ConnectionString);
        }
    }
#endif

    protected override void OnModelCreating(ModelBuilder modelBuilder) => base.OnModelCreating(modelBuilder);

    public DbSet<Core.Entities.ComunidadAutonomaHist> ComunidadesAutonomasHist { get; set; } = default!;
    public DbSet<Core.Entities.EstacionProductoPrecioHist> EstacionProductoPreciosHist { get; set; } = default!;
    public DbSet<Core.Entities.EstacionServicioHist> EstacionesServicioHist { get; set; } = default!;
    public DbSet<Core.Entities.MunicipioHist> MunicipiosHist { get; set; } = default!;
    public DbSet<Core.Entities.ProductoPetroliferoHist> ProductosPetroliferosHist { get; set; } = default!;
    public DbSet<Core.Entities.ProvinciaHist> ProvinciasHist { get; set; } = default!;

    public DbSet<Core.Entities.ProductPriceHist> ProductPriceHistView { get; set; } = default!;
}
