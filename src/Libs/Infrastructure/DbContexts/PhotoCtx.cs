using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.DbContexts;

public partial class PhotoCtx : DbContextBase
{
#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        ConfigureDb(optionsBuilder, "../databases/photo.sqlite3");
    }
#endif

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public DbSet<Core.Entities.Photo> Photos { get; set; } = default!;

    public DbSet<Core.Entities.Tag> Tags { get; set; } = default!;
}
