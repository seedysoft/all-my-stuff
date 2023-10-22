dotnet ef migrations add FirstEmpty -c DbCxt -p .\CommonLibs\Seedysoft.InfrastructureLib\Seedysoft.InfrastructureLib.csproj -s .\HomeCloud\Server\Seedysoft.HomeCloud.Server.csproj --no-build -v

dotnet ef migrations list -c CarburantesDbContext -p .\Carburantes\Infrastructure\Seedysoft.Carburantes.Infrastructure.csproj --no-build -v
