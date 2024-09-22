**Install tools**
```PowerShell
dotnet tool update --global dotnet-ef
```

**Add a migration:**
```PowerShell
dotnet ef migrations add Creating -v -c DbCxt               -o .\Migrations                -p .\src\Libs\Infrastructure\Seedysoft.Libs.Infrastructure.csproj
dotnet ef migrations add Creating -v -c GasStationPricesDbContext -o .\Infrastructure\Migrations -p .\src\Libs\GasStationPrices\Seedysoft.Libs.GasStationPrices.csproj

Add-Migration Creating -Verbose -Context DbCxt               -OutputDir .\Migrations                -Project Libs\Seedysoft.Libs.Infrastructure -StartupProject Libs\Seedysoft.Libs.Infrastructure
Add-Migration Creating -Verbose -Context GasStationPricesDbContext -OutputDir .\Infrastructure\Migrations -Project Libs\Seedysoft.Libs.GasStationPrices     -StartupProject Libs\Seedysoft.Libs.GasStationPrices
```

**Remove latest migration:**
```PowerShell
dotnet ef migrations remove -v -p .\src\Libs\Infrastructure\Seedysoft.Libs.Infrastructure.csproj
dotnet ef migrations remove -v -p .\src\Libs\GasStationPrices\Seedysoft.Libs.GasStationPrices.csproj

Remove-Migration -Verbose -Context DbCxt               -Project Libs\Seedysoft.Libs.Infrastructure -StartupProject Libs\Seedysoft.Libs.Infrastructure
Remove-Migration -Verbose -Context GasStationPricesDbContext -Project Libs\Seedysoft.Libs.GasStationPrices     -StartupProject Libs\Seedysoft.Libs.GasStationPrices
```

**List all existent migrations:**
```PowerShell
dotnet ef migrations list --no-build -v -c DbCxt               -p .\src\Libs\Infrastructure\Seedysoft.Libs.Infrastructure.csproj
dotnet ef migrations list --no-build -v -c GasStationPricesDbContext -p .\src\Libs\GasStationPrices\Seedysoft.Libs.GasStationPrices.csproj

Get-Migration -Verbose -Context DbCxt               -Project Libs\Seedysoft.Libs.Infrastructure
Get-Migration -Verbose -Context GasStationPricesDbContext -Project GasStationPrices\Seedysoft.Libs.GasStationPrices
```

**Execute a previous migration:**
```PowerShell
dotnet ef database update Creating --no-build -v -c DbCxt               -p .\src\Libs\Infrastructure\Seedysoft.Libs.Infrastructure.csproj
dotnet ef database update Creating --no-build -v -c GasStationPricesDbContext -p .\src\Libs\GasStationPrices\Seedysoft.Libs.GasStationPrices.csproj

Update-Database Creating -Verbose -Context DbCxt               -Project Libs\Seedysoft.Libs.Infrastructure
Update-Database Creating -Verbose -Context GasStationPricesDbContext -Project GasStationPrices\Seedysoft.Libs.GasStationPrices
```
