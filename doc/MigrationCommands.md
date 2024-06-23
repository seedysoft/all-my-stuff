**Add a migration:**
```
dotnet ef migrations add FirstEmpty -c DbCxt -p .\Libs\Infrastructure\Seedysoft.Libs.Infrastructure.csproj -s .\BlazorWebApp\Server\Seedysoft.BlazorWebApp.Server.csproj -v

Add-Migration FirstEmpty -Context DbCxt -Project Libs\Infrastructure\Seedysoft.Libs.Infrastructure -StartupProject Server\Seedysoft.BlazorWebApp.Server -Verbose
Add-Migration WebDataUseHttpClient -Context DbCxt -Project Libs\Infrastructure\Seedysoft.Libs.Infrastructure -StartupProject Server\Seedysoft.BlazorWebApp.Server -Verbose
```

**Remove latest migration:**
```
Remove-Migration -Context DbCxt -Project Libs\Infrastructure\Seedysoft.Libs.Infrastructure -StartupProject Server\Seedysoft.BlazorWebApp.Server -Verbose
```

**List all existent migrations:**
```
dotnet ef migrations list -c FuelPricesDbContext -p .\FuelPrices\Lib\Seedysoft.FuelPrices.Lib.csproj --no-build -v

Get-Migration -Context FuelPricesDbContext -Project FuelPrices\Seedysoft.FuelPrices.Lib -v
```

**Execute a previous migration:**
```
dotnet ef database update InitialFuelPricesDbContext -c FuelPricesDbContext -p .\FuelPrices\Lib\Seedysoft.FuelPrices.Lib.csproj --no-build -v
```
