# Publish all projects from Acer
dotnet publish Seedysoft.All.sln -p:PublishDir=D:\_Syncthing\Seedysoft\linux-arm64\            -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc;

dotnet publish Seedysoft.All.sln -p:PublishDir=D:\_Syncthing\Seedysoft\win-x64\                -c Release  -r win-x64      -p:PublishTrimmed=false  --sc;

# Publish all projects from DESKTOP-09HMH93
dotnet publish Seedysoft.All.sln -p:PublishDir=C:\Users\ptic\Syncthing\Seedysoft\linux-arm64\  -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc;

dotnet publish Seedysoft.All.sln -p:PublishDir=C:\Users\ptic\Syncthing\Seedysoft\win-x64\      -c Release  -r win-x64      -p:PublishTrimmed=false  --sc;

# Publish all projects from Raspberrypi4
dotnet publish Seedysoft.All.sln -p:PublishDir=/mnt/syncthing/Seedysoft/linux-arm64\           -c Release  -r linux-arm64  -p:PublishTrimmed=false  --sc;

dotnet publish Seedysoft.All.sln -p:PublishDir=/mnt/syncthing/Seedysoft/win-x64\               -c Release  -r win-x64      -p:PublishTrimmed=false  --sc;


# Para ver errores podemos añadir --verbosity n

#   -c | --configuration
#   -p | --property
#   -r | --runtime
# --sc | --self-contained
