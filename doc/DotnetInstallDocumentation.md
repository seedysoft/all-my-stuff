**Dotnet tools Documentation:** \
<https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools>

**Entity Framework Core Documentation:** \
<https://learn.microsoft.com/en-us/ef/core/cli/dotnet>

**Manual installation documentation:** \
<https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#manual-install>

dotnet tool update --global PowerShell

==**NOTE: On first installation, export variables:**== \
Windows (PowerShell):
```PowerShell
[Environment]::SetEnvironmentVariable("SEEDY_MASTER_KEY", "", [System.EnvironmentVariableTarget]::User)
```

Raspberrypi ($):
```bash
sudo nano /etc/environment
```
then paste this:
```
DOTNET_ROOT=/opt/dotnet/
PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin:
prgDir=/mnt/wd/_prog/seedysoft/
pubDir=/mnt/st/Seedysoft/

SEEDY_MASTER_KEY=""
```
WSL (PowerShell):
```PowerShell
[Environment]::SetEnvironmentVariable("WSLENV", $env:WSLENV + "SEEDY_MASTER_KEY:", [System.EnvironmentVariableTarget]::User)
```

sudo apt-get install chromium-chromedriver

**Dotnet downloads:** \
<https://dotnet.microsoft.com/en-us/download/dotnet>
```bash
wget -O 
DOTNET_FILE=
```

**Extract compressed file in current directory (/opt/dotnet/):**
```bash
tar zxvf "$DOTNET_FILE" -C "$DOTNET_ROOT"
```

**Help commands:**
```bash
dotnet --list-sdks; dotnet --list-runtimes;
```

**Remove old versions:**
```bash
find . -type d -name "8.0.1" -exec rm -rf "{}" \;
```

**Install workloads:** \
==NOTE: Must be root (sudo su)==: *root@raspberrypi4:/opt/dotnet#*
```bash
# ./dotnet workload search [<SEARCH_STRING>] [-v|--verbosity <LEVEL>]
# ./dotnet workload search -?|-h|--help
dotnet workload install wasm-tools
dotnet workload update
```

**Update Entity Framework global tools:**
```bash
dotnet tool update --global dotnet-ef
```

**MudBlazor Templates
```bash
dotnet new install MudBlazor.Templates
```