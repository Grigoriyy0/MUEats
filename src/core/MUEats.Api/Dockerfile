FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY --chmod=644 ./src/core/MUEats.Api/private_key.pem /app/private_key.pem
COPY ["src/core/MUEats.Api/MUEats.Api.csproj", "src/core/MUEats.Api/"]
COPY ["src/core/MUEats.Infrastructure/MUEats.Infrastructure.csproj", "src/core/MUEats.Infrastructure/"]
COPY ["src/core/MUEats.Application/MUEats.Application.csproj", "src/core/MUEats.Application/"]
COPY ["src/core/MUEats.Core/MUEats.Core.csproj", "src/core/MUEats.Core/"]
RUN dotnet restore "src/core/MUEats.Api/MUEats.Api.csproj"
COPY . .
WORKDIR "/src/src/core/MUEats.Api"
RUN dotnet build "./MUEats.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MUEats.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./src/core/MUEats.Api/private_key.pem /app/private_key.pem
ENTRYPOINT ["dotnet", "MUEats.Api.dll"]
