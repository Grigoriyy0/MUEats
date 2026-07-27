FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY --chmod=644 ./src/restaurants/MUEats.Restaurants.Api/public.key /app/public.key
COPY ["src/restaurants/MUEats.Restaurants.Api/MUEats.Restaurants.Api.csproj", "src/restaurants/MUEats.Restaurants.Api/"]
COPY ["src/restaurants/MUEats.Restaurants.Application/MUEats.Restaurants.Application.csproj", "src/restaurants/MUEats.Restaurants.Application/"]
COPY ["src/restaurants/MUEats.Restaurants.Core/MUEats.Restaurants.Core.csproj", "src/restaurants/MUEats.Restaurants.Core/"]
COPY ["src/utils/Primitives/Primitives.csproj", "src/utils/Primitives/"]
COPY ["src/restaurants/MUEats.Restaurants.Infrastructure/MUEats.Restaurants.Infrastructure.csproj", "src/restaurants/MUEats.Restaurants.Infrastructure/"]
RUN dotnet restore "src/restaurants/MUEats.Restaurants.Api/MUEats.Restaurants.Api.csproj"
COPY . .
WORKDIR "/src/src/restaurants/MUEats.Restaurants.Api"
RUN dotnet build "./MUEats.Restaurants.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MUEats.Restaurants.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./src/restaurants/MUEats.Restaurants.Api/public.key /app/public.key
ENTRYPOINT ["dotnet", "MUEats.Restaurants.Api.dll"]
