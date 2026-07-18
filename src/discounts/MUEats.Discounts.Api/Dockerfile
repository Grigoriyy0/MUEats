FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/discounts/MUEats.Discounts.Api/MUEats.Discounts.Api.csproj", "src/discounts/MUEats.Discounts.Api/"]
COPY ["src/discounts/MUEats.Discounts.Infrastructure/MUEats.Discounts.Infrastructure.csproj", "src/discounts/MUEats.Discounts.Infrastructure/"]
COPY ["src/discounts/MUEats.Discounts.Application/MUEats.Discounts.Application.csproj", "src/discounts/MUEats.Discounts.Application/"]
COPY ["src/discounts/MUEats.Discounts.Core/MUEats.Discounts.Core.csproj", "src/discounts/MUEats.Discounts.Core/"]
COPY ["src/utils/Primitives/Primitives.csproj", "src/utils/Primitives/"]
COPY ["src/utils/SharedContracts/SharedContracts.csproj", "src/utils/SharedContracts/"]
RUN dotnet restore "src/discounts/MUEats.Discounts.Api/MUEats.Discounts.Api.csproj"
COPY . .
WORKDIR "/src/src/discounts/MUEats.Discounts.Api"
RUN dotnet build "./MUEats.Discounts.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MUEats.Discounts.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MUEats.Discounts.Api.dll"]
