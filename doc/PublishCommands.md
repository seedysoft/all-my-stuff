**Publish all projects from Acer:**
```
dotnet publish Seedysoft.Tests.sln -p:PublishDir=D:\\_Syncthing\Seedysoft\linux-arm64\  -c Release  -r linux-arm64  -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;
dotnet publish Seedysoft.Tests.sln -p:PublishDir=D:\\_Syncthing\Seedysoft\win-x64\      -c Release  -r win-x64      -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;

dotnet publish C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\Carburantes\ConsoleApp\Seedysoft.Carburantes.ConsoleApp.csproj -p:PublishDir=D:\\_Syncthing\Seedysoft\linux-arm64\  -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc ;
dotnet publish C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\Outbox\OutboxConsoleApp\Seedysoft.OutboxConsoleApp.csproj -p:PublishDir=D:\\_Syncthing\Seedysoft\linux-arm64\  -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc ;
dotnet publish C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\Pvpc\PvpcConsoleApp\Seedysoft.PvpcConsoleApp.csproj -p:PublishDir=D:\\_Syncthing\Seedysoft\linux-arm64\  -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc ;
dotnet publish C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\WebComparer\WebComparerConsoleApp\Seedysoft.WebComparerConsoleApp.csproj -p:PublishDir=D:\\_Syncthing\Seedysoft\linux-arm64\  -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc ;
dotnet publish C:\Users\alfon\_prog\seedysoft\all-my-stuff\src\BlazorWebApp\Server\Seedysoft.BlazorWebApp.Server.csproj -p:PublishDir=D:\\_Syncthing\Seedysoft\linux-arm64\  -c Release  -r linux-arm64  -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;
```

**Publish all projects from Raspberrypi4:**
```
dotnet publish /mnt/wd/_prog/seedysoft/all-my-stuff/Seedysoft.Test.sln -p:PublishDir=/mnt/st/Seedysoft/linux-arm64/ -c Release  -r linux-arm64  -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;
cd /mnt/st/Seedysoft/linux-arm64 ;
bash create-HomeCloudServer-daemon.sh ;

dotnet publish /mnt/wd/_prog/seedysoft/all-my-stuff/Seedysoft.Test.sln -p:PublishDir=/mnt/st/Seedysoft/win-x64      -c Release  -r win-x64      -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;
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
