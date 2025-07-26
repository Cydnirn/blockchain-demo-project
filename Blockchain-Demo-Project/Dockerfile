FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Blockchain-Demo-Project/Blockchain-Demo-Project.csproj", "Blockchain-Demo-Project/"]
RUN dotnet restore "Blockchain-Demo-Project/Blockchain-Demo-Project.csproj"
COPY . .
WORKDIR "/src/Blockchain-Demo-Project"
RUN dotnet build "./Blockchain-Demo-Project.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Blockchain-Demo-Project.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blockchain-Demo-Project.dll"]
