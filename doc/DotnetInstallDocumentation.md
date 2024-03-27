**Dotnet downloads:** \
<https://dotnet.microsoft.com/en-us/download/dotnet>

**Manual installation documentation:** \
<https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#manual-install>

==**NOTE: On first installation, export variables:**== \
<https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#set-environment-variables-system-wide>

**Extract compressed file in current directory (/opt/dotnet/):** \
`tar zxfv dotnet... -C "$DOTNET_ROOT"`

**Help commands:** \
`dotnet --list-sdks` \
`dotnet --list-runtimes`

**Remove old versions:** \
`find . -type d -name "8.0.1" -exec rm -rf "{}" \;`

**Install workloads:** \
==NOTE: Must be root (sudo su)==: *root@raspberrypi4:/opt/dotnet#* \
`./dotnet workload search [<SEARCH_STRING>] [-v|--verbosity <LEVEL>]` \
`./dotnet workload search -?|-h|--help` \
`./dotnet workload install wasm-tools`
