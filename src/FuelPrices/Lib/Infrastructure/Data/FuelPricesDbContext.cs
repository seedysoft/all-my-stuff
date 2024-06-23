using Microsoft.EntityFrameworkCore;
using Seedysoft.FuelPrices.Lib.Core.Entities;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Data;

public sealed class FuelPricesDbContext : DbContext
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

    public DbSet<ComunidadAutonoma> ComunidadesAutonomas { get; set; }
    public DbSet<EstacionProductoPrecio> EstacionProductoPrecios { get; set; } = default!;
    public DbSet<EstacionServicio> EstacionesServicio { get; set; } = default!;
    public DbSet<Municipio> Municipios { get; set; } = default!;
    public DbSet<ProductoPetrolifero> ProductosPetroliferos { get; set; } = default!;
    public DbSet<Provincia> Provincias { get; set; } = default!;

    public DbSet<ProductPrice> ProductPriceView { get; set; } = default!;
}
