using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.DbContexts;

public sealed partial class DbRpt : DbContextBase
{
#if DEBUG
    public DbRpt() : base() { }
#endif
    public DbRpt(DbContextOptions<DbRpt> options) : base(options) { }

#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
#endif
            string DatabasePath = "../databases/rpt.sqlite3";
            string FullFilePath = Path.GetFullPath(DatabasePath);
            while (!File.Exists(FullFilePath))
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
                FullFilePath = Path.GetFullPath(DatabasePath = DatabasePath.Insert(0, "../"));
            }

            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException($"Database file for {nameof(DbRpt)} not found.", FullFilePath);

            string ConnectionString = $"{Core.Constants.DatabaseStrings.DataSource}{FullFilePath}";
            Console.WriteLine(ConnectionString);

            _ = optionsBuilder.UseSqlite(ConnectionString);
        }
    }
#endif

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly, t => t.FullName?.Contains(nameof(EntityTypeConfigurations.DbRpt)) ?? false);
    }

    public DbSet<Core.Entities.CentroDirectivo> CentrosDirectivos { get; set; } = default!;

    public DbSet<Core.Entities.Localidad> Localidades { get; set; } = default!;

    public DbSet<Core.Entities.Ministerio> Ministerios { get; set; } = default!;

    public DbSet<Core.Entities.Pais> Paises { get; set; } = default!;

    public DbSet<Core.Entities.Provincia> Provincias { get; set; } = default!;

    public DbSet<Core.Entities.Puesto> Puestos { get; set; } = default!;

    public DbSet<Core.Entities.Unidad> Unidades { get; set; } = default!;
}
