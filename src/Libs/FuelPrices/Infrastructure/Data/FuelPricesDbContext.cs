using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.FuelPrices.Infrastructure.Data;

internal sealed class FuelPricesDbContext : DbContext
{
    public FuelPricesDbContext(DbContextOptions options) : base(options) => ChangeTracker.LazyLoadingEnabled = false;

#if DEBUG
    public FuelPricesDbContext() : base() { }

    protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
    {
        base.OnConfiguring(dbContextOptionsBuilder);

        if (!dbContextOptionsBuilder.IsConfigured)
        {
            string ConnectionString = "Data Source=InMemorySample;Mode=Memory;Cache=Shared";
            _ = dbContextOptionsBuilder.UseSqlite(ConnectionString);
        }
    }
#endif

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public DbSet<Core.Entities.ComunidadAutonoma> ComunidadesAutonomas { get; set; }
    public DbSet<Core.Entities.EstacionProductoPrecio> EstacionProductoPrecios { get; set; } = default!;
    public DbSet<Core.Entities.EstacionServicio> EstacionesServicio { get; set; } = default!;
    public DbSet<Core.Entities.Municipio> Municipios { get; set; } = default!;
    public DbSet<Core.Entities.ProductoPetrolifero> ProductosPetroliferos { get; set; } = default!;
    public DbSet<Core.Entities.Provincia> Provincias { get; set; } = default!;

    public DbSet<Core.Entities.ProductPrice> ProductPriceView { get; set; } = default!;
}
