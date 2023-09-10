#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Carburantes/ConsoleApp/Seedysoft.Carburantes.ConsoleApp.csproj", "Carburantes/ConsoleApp/"]
COPY ["CommonLibs/UtilsLib/Seedysoft.UtilsLib.csproj", "CommonLibs/UtilsLib/"]
COPY ["Carburantes/Infrastructure/Seedysoft.Carburantes.Infrastructure.csproj", "Carburantes/Infrastructure/"]
COPY ["Carburantes/Core/Seedysoft.Carburantes.Core.csproj", "Carburantes/Core/"]
COPY ["CommonLibs/CronBackgroundServiceLib/Seedysoft.CronBackgroundServiceLib.csproj", "CommonLibs/CronBackgroundServiceLib/"]
COPY ["Carburantes/Services/Seedysoft.Carburantes.Services.csproj", "Carburantes/Services/"]
RUN dotnet restore "Carburantes/ConsoleApp/Seedysoft.Carburantes.ConsoleApp.csproj"
COPY . .
WORKDIR "/src/Carburantes/ConsoleApp"
RUN dotnet build "Seedysoft.Carburantes.ConsoleApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seedysoft.Carburantes.ConsoleApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seedysoft.Carburantes.ConsoleApp.dll"]