# Publish Commands

## Publish all projects from Acer
    dotnet publish Seedysoft.sln -p:PublishDir=D:\\_Syncthing\Seedysoft\linux-arm64\  -c Release  -r linux-arm64  -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;

    dotnet publish Seedysoft.sln -p:PublishDir=D:\\_Syncthing\Seedysoft\win-x64\      -c Release  -r win-x64      -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;

## Publish all projects from Raspberrypi4
    dotnet publish /mnt/wd/_prog/seedysoft/all-my-stuff/Seedysoft.sln -p:PublishDir=/mnt/st/Seedysoft/linux-arm64/ -c Release  -r linux-arm64  -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ; \
    cd /mnt/st/Seedysoft/linux-arm64 ; \
    bash create-HomeCloudServer-daemon.sh ;

    dotnet publish /mnt/wd/_prog/seedysoft/all-my-stuff/Seedysoft.sln -p:PublishDir=/mnt/st/Seedysoft/win-x64      -c Release  -r win-x64      -p:BlazorCacheBootResources=false  -p:PublishTrimmed=false  --sc ;

### Para ver errores podemos añadir --verbosity

#### Explicación de las opciones:
    -c | --configuration
    -p | --property
    -r | --runtime
    --sc | --self-contained
