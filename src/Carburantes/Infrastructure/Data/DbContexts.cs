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
    public CarburantesDbContext(DbContextOptions<CarburantesDbContext> options) : base(options) { }

#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string ConnectionString = $"{CoreLib.Constants.DatabaseStrings.DataSource}../../../../databases/Carburantes.sqlite3";
            Console.WriteLine(ConnectionString);
            string FullFilePath = Path.GetFullPath(ConnectionString[CoreLib.Constants.DatabaseStrings.DataSource.Length..]);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

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
}

public sealed class CarburantesHistDbContext : CarburantesDbContextBase
{
#if DEBUG
    public CarburantesHistDbContext() : base() { }
#endif
    public CarburantesHistDbContext(DbContextOptions<CarburantesHistDbContext> options) : base(options) { }

#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string ConnectionString = $"{CoreLib.Constants.DatabaseStrings.DataSource}../../../../databases/CarburantesHist.sqlite3";
            Console.WriteLine(ConnectionString);
            string FullFilePath = Path.GetFullPath(ConnectionString[CoreLib.Constants.DatabaseStrings.DataSource.Length..]);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

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
}
