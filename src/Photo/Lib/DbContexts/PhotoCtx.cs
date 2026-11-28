using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Photo.Lib.DbContexts;

public partial class PhotoCtx : Libs.Infrastructure.DbContexts.DbContextBase
{
    public PhotoCtx() : base() { }
    public PhotoCtx(DbContextOptions options) : base(options) { }

    public DbSet<Entities.Photo> Photos { get; set; } = default!;

    public DbSet<Entities.Tag> Tags { get; set; } = default!;
}
