# Migration Commands

## Add a migration
    dotnet ef migrations add FirstEmpty -c DbCxt -p .\CommonLibs\Seedysoft.InfrastructureLib\Seedysoft.InfrastructureLib.csproj -s .\HomeCloud\Server\Seedysoft.HomeCloud.Server.csproj --no-build -v
    Add-Migration FirstEmpty -Context DbCxt -Project CommonLibs\Seedysoft.InfrastructureLib -StartupProject HomeCloud\Seedysoft.HomeCloud.Server -Verbose
    Add-Migration WebDataUseHttpClient -Context DbCxt -Project CommonLibs\Seedysoft.InfrastructureLib -StartupProject HomeCloud\Seedysoft.HomeCloud.Server -Verbose

## Remove latest migration
    Remove-Migration -Context DbCxt -Project CommonLibs\Seedysoft.InfrastructureLib -StartupProject HomeCloud\Seedysoft.HomeCloud.Server -Verbose

## List all existent migrations
    dotnet ef migrations list -c CarburantesDbContext -p .\Carburantes\Infrastructure\Seedysoft.Carburantes.Infrastructure.csproj --no-build -v

## Execute a previous migration
    dotnet ef database update InitialCarburantesDbContext -c CarburantesDbContext -p .\Carburantes\Infrastructure\Seedysoft.Carburantes.Infrastructure.csproj --no-build -v
