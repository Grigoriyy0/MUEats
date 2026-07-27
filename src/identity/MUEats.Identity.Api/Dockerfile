FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY --chmod=644 ./src/identity/MUEats.Identity.Api/private.key /app/private.key
COPY ["src/identity/MUEats.Identity.Api/MUEats.Identity.Api.csproj", "src/identity/MUEats.Identity.Api/"]
COPY ["src/identity/MUEats.Identity.Application/MUEats.Identity.Application.csproj", "src/identity/MUEats.Identity.Application/"]
COPY ["src/identity/MUEats.Identity.Core/MUEats.Identity.Core.csproj", "src/identity/MUEats.Identity.Core/"]
COPY ["src/utils/Primitives/Primitives.csproj", "src/utils/Primitives/"]
COPY ["src/identity/MUEats.Identity.Infrastructure/MUEats.Identity.Infrastructure.csproj", "src/identity/MUEats.Identity.Infrastructure/"]
RUN dotnet restore "src/identity/MUEats.Identity.Api/MUEats.Identity.Api.csproj"
COPY . .
WORKDIR "/src/src/identity/MUEats.Identity.Api"
RUN dotnet build "./MUEats.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MUEats.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./src/identity/MUEats.Identity.Api/private.key /app/private.key
ENTRYPOINT ["dotnet", "MUEats.Identity.Api.dll"]
