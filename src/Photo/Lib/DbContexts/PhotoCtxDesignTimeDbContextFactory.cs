namespace Seedysoft.Photo.Lib.DbContexts;

public class PhotoCtxDesignTimeDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<PhotoCtx>
{
    public PhotoCtx CreateDbContext(string[] args)
    {
        Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<PhotoCtx> dbContextOptionsBuilder
            = Libs.Infrastructure.DbContexts.DbContextBase.ConfigureDb<PhotoCtx>(null, Path.Join(Directory.GetCurrentDirectory(), "../../databases/photo.sqlite3"));

        return new PhotoCtx(dbContextOptionsBuilder.Options);
    }
}
