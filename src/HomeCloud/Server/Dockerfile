#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Server/Seedysoft.HomeCloud.Server.csproj", "Server/"]
COPY ["Client/Seedysoft.HomeCloud.Client.csproj", "Client/"]
COPY ["Shared/Seedysoft.HomeCloud.Shared.csproj", "Shared/"]
RUN dotnet restore "Server/Seedysoft.HomeCloud.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "Seedysoft.HomeCloud.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seedysoft.HomeCloud.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seedysoft.HomeCloud.Server.dll"]