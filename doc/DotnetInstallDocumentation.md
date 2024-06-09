**Dotnet tools Documentation:** \
<https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools>

**Entity Framework Core Documentation:** \
<https://learn.microsoft.com/en-us/ef/core/cli/dotnet>

**Manual installation documentation:** \
<https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#manual-install>

==**NOTE: On first installation, export variables:**== \
Windows (PowerShell): \
`[Environment]::SetEnvironmentVariable("SEEDY_MASTER_KEY", "", [System.EnvironmentVariableTarget]::User)`

Raspberrypi ($): \
`sudo nano /etc/profile.d/environment.sh`
```
export DOTNET_ROOT=/opt/dotnet/
export PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin:
export prgDir=/mnt/wd/_prog/seedysoft/
export pubDir=/mnt/st/Seedysoft/

export SEEDY_MASTER_KEY=""
```
WSL (PowerShell): \
`[Environment]::SetEnvironmentVariable("WSLENV", $env:WSLENV + "SEEDY_MASTER_KEY:", [System.EnvironmentVariableTarget]::User)`

**Dotnet downloads:** \
<https://dotnet.microsoft.com/en-us/download/dotnet>
```
wget -O 
DOTNET_FILE=
```
**Extract compressed file in current directory (/opt/dotnet/):** \
`tar zxvf "$DOTNET_FILE" -C "$DOTNET_ROOT"`

**Help commands:** \
`dotnet --list-sdks; dotnet --list-runtimes;`

**Remove old versions:** \
`find . -type d -name "8.0.1" -exec rm -rf "{}" \;`

**Install workloads:** \
==NOTE: Must be root (sudo su)==: *root@raspberrypi4:/opt/dotnet#*
```
./dotnet workload search [<SEARCH_STRING>] [-v|--verbosity <LEVEL>]
./dotnet workload search -?|-h|--help
./dotnet workload install wasm-tools
```
**Update Entity Framework global tools:** \
`dotnet tool update --global dotnet-ef`