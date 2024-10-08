﻿**Publish all projects from BestiaParda:**
```PowerShell
$ProjectOrSolution="C:\Users\alfon\_prog\seedysoft\all-my-stuff\Seedysoft.Tests.sln"
$ProjectOrSolution="C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\BlazorWebApp\Server\Seedysoft.BlazorWebApp.Server.csproj"

dotnet publish $ProjectOrSolution -p:PublishDir=C:\Users\alfon\_prog\seedysoft\all-my-stuff\publish\linux-arm64\  -c Release  -r linux-arm64  -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  -p:PublishReadyToRun  -p:PublishReadyToRunShowWarnings=true  --sc ;
dotnet publish $ProjectOrSolution -p:PublishDir=C:\Users\alfon\_prog\seedysoft\all-my-stuff\publish\win-x64\      -c Release  -r win-x64      -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;

dotnet publish $ProjectOrSolution -p:PublishDir=C:\Users\alfon\_prog\seedysoft\Test\linux-x64\  -c Debug    -r linux-x64  -p:EnvironmentName="Development"  -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;
dotnet publish $ProjectOrSolution -p:PublishDir=C:\Users\alfon\_prog\seedysoft\Test\linux-x64\  -c Release  -r linux-x64                                    -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;

dotnet publish C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\Outbox\ConsoleApp\Seedysoft.Outbox.ConsoleApp.csproj            -p:PublishDir=C:\Users\alfon\_prog\seedysoft\all-my-stuff\publish\linux-arm64\  -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc ;
dotnet publish C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\Pvpc\ConsoleApp\Seedysoft.Pvpc.ConsoleApp.csproj                -p:PublishDir=C:\Users\alfon\_prog\seedysoft\all-my-stuff\publish\linux-arm64\  -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc ;
dotnet publish C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\WebComparer\ConsoleApp\Seedysoft.WebComparer.ConsoleApp.csproj  -p:PublishDir=C:\Users\alfon\_prog\seedysoft\all-my-stuff\publish\linux-arm64\  -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc ;
dotnet publish C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\BlazorWebApp\Server\Seedysoft.BlazorWebApp.Server.csproj        -p:PublishDir=C:\Users\alfon\_prog\seedysoft\all-my-stuff\publish\linux-arm64\  -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc -p:BlazorCacheBootResources=false ;
```

**Publish all projects from Acer:**
```PowerShell
$ProjectOrSolution="C:\Users\alfon\_prog\seedysoft\all-my-stuff\Seedysoft.Tests.sln"
$ProjectOrSolution="C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\BlazorWebApp\Server\Seedysoft.BlazorWebApp.Server.csproj"

dotnet publish $ProjectOrSolution -p:PublishDir=D:\\_Syncthing\Seedysoft\linux-arm64\  -c Release  -r linux-arm64  -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;
dotnet publish $ProjectOrSolution -p:PublishDir=D:\\_Syncthing\Seedysoft\win-x64\      -c Release  -r win-x64      -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;

dotnet publish $ProjectOrSolution -p:PublishDir=D:\\Test\linux-x64\  -c Debug  -r linux-x64  -p:EnvironmentName="Development"  -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;
dotnet publish $ProjectOrSolution -p:PublishDir=D:\\Test\linux-x64\  -c Release  -r linux-x64    -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;

dotnet publish C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\Outbox\ConsoleApp\Seedysoft.Outbox.ConsoleApp.csproj            -p:PublishDir=D:\\_Syncthing\Seedysoft\linux-arm64\  -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc ;
dotnet publish C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\Pvpc\ConsoleApp\Seedysoft.Pvpc.ConsoleApp.csproj                -p:PublishDir=D:\\_Syncthing\Seedysoft\linux-arm64\  -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc ;
dotnet publish C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\WebComparer\ConsoleApp\Seedysoft.WebComparer.ConsoleApp.csproj  -p:PublishDir=D:\\_Syncthing\Seedysoft\linux-arm64\  -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc ;
dotnet publish C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\BlazorWebApp\Server\Seedysoft.BlazorWebApp.Server.csproj        -p:PublishDir=D:\\_Syncthing\Seedysoft\linux-arm64\  -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc -p:BlazorCacheBootResources=false ;
```

**Publish all projects from Raspberrypi4:**
```bash
dotnet publish /mnt/wd/_prog/seedysoft/all-my-stuff/Seedysoft.Test.sln -p:PublishDir=/mnt/st/Seedysoft/linux-arm64/  -c Release  -r linux-arm64  -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;
cd /mnt/st/Seedysoft/linux-arm64 ;
bash create-BlazorWebApp-daemon.sh ;

dotnet publish /mnt/wd/_prog/seedysoft/all-my-stuff/Seedysoft.Test.sln -p:PublishDir=/mnt/st/Seedysoft/win-x64       -c Release  -r win-x64      -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;
```

Para ver errores podemos añadir --verbosity

**Explicación de las opciones:**
```
 -c  | --configuration
 -p  | --property
 -r  | --runtime
--sc | --self-contained
 -v  | --verbosity <LEVEL> # Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]. The default is minimal.
```
