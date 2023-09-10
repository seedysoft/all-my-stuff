#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Outbox/OutboxConsoleApp/Seedysoft.OutboxConsoleApp.csproj", "Outbox/OutboxConsoleApp/"]
COPY ["CommonLibs/SmtpServiceLib/Seedysoft.SmtpServiceLib.csproj", "CommonLibs/SmtpServiceLib/"]
COPY ["CommonLibs/TelegramLib/Seedysoft.TelegramLib.csproj", "CommonLibs/TelegramLib/"]
COPY ["ObtainPvpc/ObtainPvpcLib/Seedysoft.ObtainPvpcLib.csproj", "ObtainPvpc/ObtainPvpcLib/"]
COPY ["CommonLibs/CoreLib/Seedysoft.CoreLib.csproj", "CommonLibs/CoreLib/"]
COPY ["CommonLibs/UtilsLib/Seedysoft.UtilsLib.csproj", "CommonLibs/UtilsLib/"]
COPY ["CommonLibs/CronBackgroundServiceLib/Seedysoft.CronBackgroundServiceLib.csproj", "CommonLibs/CronBackgroundServiceLib/"]
COPY ["CommonLibs/Seedysoft.InfrastructureLib/Seedysoft.InfrastructureLib.csproj", "CommonLibs/Seedysoft.InfrastructureLib/"]
RUN dotnet restore "Outbox/OutboxConsoleApp/Seedysoft.OutboxConsoleApp.csproj"
COPY . .
WORKDIR "/src/Outbox/OutboxConsoleApp"
RUN dotnet build "Seedysoft.OutboxConsoleApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seedysoft.OutboxConsoleApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seedysoft.OutboxConsoleApp.dll"]