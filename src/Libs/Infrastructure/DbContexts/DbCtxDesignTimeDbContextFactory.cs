namespace Seedysoft.Libs.Infrastructure.DbContexts;

public class DbCtxDesignTimeDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<DbCtx>
{
    public DbCtx CreateDbContext(string[] args)
    {
        Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<DbCtx> optionsBuilder
            = DbContextBase.ConfigureDb<DbCtx>(null, Path.Join(Directory.GetCurrentDirectory(), "../../databases/db.sqlite3"));

        return new DbCtx(optionsBuilder.Options);
    }
}
