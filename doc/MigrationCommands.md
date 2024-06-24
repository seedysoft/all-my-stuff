**Install tools**
```PowerShell
dotnet tool update --global dotnet-ef
```

**Add a migration:**
```PowerShell
dotnet ef migrations add Creating -v -c DbCxt               -o .\Migrations                -p .\src\Libs\Infrastructure\Seedysoft.Libs.Infrastructure.csproj
dotnet ef migrations add Creating -v -c FuelPricesDbContext -o .\Infrastructure\Migrations -p .\src\FuelPrices\Lib\Seedysoft.FuelPrices.Lib.csproj

Add-Migration Creating -Verbose -Context DbCxt               -OutputDir .\Migrations                -Project Libs\Seedysoft.Libs.Infrastructure
Add-Migration Creating -Verbose -Context FuelPricesDbContext -OutputDir .\Infrastructure\Migrations -Project FuelPrices\Seedysoft.FuelPrices.Lib
```

**Remove latest migration:**
```PowerShell
dotnet ef migrations remove -v -p .\src\Libs\Infrastructure\Seedysoft.Libs.Infrastructure.csproj
dotnet ef migrations remove -v -p .\src\FuelPrices\Lib\Seedysoft.FuelPrices.Lib.csproj

Remove-Migration -Verbose -Context DbCxt               -Project Libs\Infrastructure\Seedysoft.Libs.Infrastructure
Remove-Migration -Verbose -Context FuelPricesDbContext -Project FuelPrices\Seedysoft.FuelPrices.Lib
```

**List all existent migrations:**
```PowerShell
dotnet ef migrations list --no-build -v -c DbCxt               -p .\src\Libs\Infrastructure\Seedysoft.Libs.Infrastructure.csproj
dotnet ef migrations list --no-build -v -c FuelPricesDbContext -p .\src\FuelPrices\Lib\Seedysoft.FuelPrices.Lib.csproj

Get-Migration -Verbose -Context DbCxt               -Project Libs\Seedysoft.Libs.Infrastructure
Get-Migration -Verbose -Context FuelPricesDbContext -Project FuelPrices\Seedysoft.FuelPrices.Lib
```

**Execute a previous migration:**
```PowerShell
dotnet ef database update Creating --no-build -v -c DbCxt               -p .\src\Libs\Infrastructure\Seedysoft.Libs.Infrastructure.csproj
dotnet ef database update Creating --no-build -v -c FuelPricesDbContext -p .\src\FuelPrices\Lib\Seedysoft.FuelPrices.Lib.csproj

Update-Database Creating -Verbose -Context DbCxt               -Project Libs\Seedysoft.Libs.Infrastructure
Update-Database Creating -Verbose -Context FuelPricesDbContext -Project FuelPrices\Seedysoft.FuelPrices.Lib
```
