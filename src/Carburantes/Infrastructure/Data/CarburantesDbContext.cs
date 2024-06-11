using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Carburantes.Infrastructure.Data;

public sealed class CarburantesDbContext : DbContext
{
#if DEBUG
    public CarburantesDbContext() : base() { }
#endif

#if !DEBUG
#pragma warning disable IDE0290 // Use primary constructor
#endif
    public CarburantesDbContext(DbContextOptions<CarburantesDbContext> options) : base(options) => ChangeTracker.LazyLoadingEnabled = false;
#if !DEBUG
#pragma warning restore IDE0290 // Use primary constructor
#endif

#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string DatabasePath = "../../../../../../../../databases/Carburantes.{yyMM}.sqlite3".Replace("{yyMM}", DateTime.Today.ToUniversalTime().ToString("yyMM"));
            string FullFilePath = Path.GetFullPath(DatabasePath);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            string ConnectionString = $"{CoreLib.Constants.DatabaseStrings.DataSource}{FullFilePath}";
            Console.WriteLine(ConnectionString);

            _ = optionsBuilder.UseSqlite(ConnectionString);
        }
    }
#endif

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
    }

    public DbSet<Core.Entities.ComunidadAutonoma> ComunidadesAutonomas { get; set; } = default!;
    public DbSet<Core.Entities.EstacionProductoPrecio> EstacionProductoPrecios { get; set; } = default!;
    public DbSet<Core.Entities.EstacionServicio> EstacionesServicio { get; set; } = default!;
    public DbSet<Core.Entities.Municipio> Municipios { get; set; } = default!;
    public DbSet<Core.Entities.ProductoPetrolifero> ProductosPetroliferos { get; set; } = default!;
    public DbSet<Core.Entities.Provincia> Provincias { get; set; } = default!;

    public DbSet<Core.Entities.ProductPrice> ProductPriceView { get; set; } = default!;
}
