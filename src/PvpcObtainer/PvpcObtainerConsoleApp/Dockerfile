#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PvpcObtainer/PvpcObtainerConsoleApp/Seedysoft.PvpcObtainerConsoleApp.csproj", "PvpcObtainer/PvpcObtainerConsoleApp/"]
COPY ["CommonLibs/UtilsLib/Seedysoft.UtilsLib.csproj", "CommonLibs/UtilsLib/"]
COPY ["PvpcObtainer/PvpcObtainerLib/Seedysoft.PvpcObtainerLib.csproj", "PvpcObtainer/PvpcObtainerLib/"]
COPY ["CommonLibs/CoreLib/Seedysoft.CoreLib.csproj", "CommonLibs/CoreLib/"]
COPY ["CommonLibs/CronBackgroundServiceLib/Seedysoft.CronBackgroundServiceLib.csproj", "CommonLibs/CronBackgroundServiceLib/"]
COPY ["CommonLibs/Seedysoft.InfrastructureLib/Seedysoft.InfrastructureLib.csproj", "CommonLibs/Seedysoft.InfrastructureLib/"]
RUN dotnet restore "PvpcObtainer/PvpcObtainerConsoleApp/Seedysoft.PvpcObtainerConsoleApp.csproj"
COPY . .
WORKDIR "/src/PvpcObtainer/PvpcObtainerConsoleApp"
RUN dotnet build "Seedysoft.PvpcObtainerConsoleApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seedysoft.PvpcObtainerConsoleApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seedysoft.PvpcObtainerConsoleApp.dll"]