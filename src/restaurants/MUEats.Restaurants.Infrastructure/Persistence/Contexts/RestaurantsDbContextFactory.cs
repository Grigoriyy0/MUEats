using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MUEats.Restaurants.Infrastructure.Persistence.Contexts;

/// dotnet ef migrations add / database update по умолчанию пытаются
/// поднять весь Host приложения (Program.cs), чтобы достать оттуда
/// настроенный DbContext — а это запускает и Kafka-консьюмеры, и
/// другие фоновые сервисы, которые недоступны при запуске CLI прямо на
/// Mac (не внутри Docker), и виснут на 5 минут до таймаута.
///
/// Эта фабрика даёт dotnet ef отдельный, лёгкий способ создать
/// DbContext — напрямую, без Program.cs и без всего остального хоста.
/// dotnet ef находит её сам через reflection, ничего регистрировать не
/// нужно.
///
/// ВАЖНО: .UseSnakeCaseNamingConvention() здесь обязателен — настоящее
/// приложение настроено именно так (см.
/// MUEats.Restaurants.Infrastructure/DependencyInjection.cs), и без
/// этого EF Core будет генерировать запросы в PascalCase ("MigrationId"
/// вместо реального "migration_id"), что и вызывало ошибку 42703.
///
/// Порт 5434 — это host-порт restaurants-postgres из docker-compose
/// (контейнер слушает 5432 внутри, но наружу на Mac смотрит как 5434).
public class RestaurantsDbContextFactory : IDesignTimeDbContextFactory<RestaurantsDbContext>
{
    public RestaurantsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RestaurantsDbContext>();
        optionsBuilder
            .UseNpgsql("Host=localhost;Port=5434;Username=restaurants-user;Password=restaurantspassword;Database=mue.restaurants")
            .UseSnakeCaseNamingConvention();

        return new RestaurantsDbContext(optionsBuilder.Options);
    }
}