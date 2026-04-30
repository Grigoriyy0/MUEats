using Microsoft.EntityFrameworkCore;
using MUEats.Restaurants.Core.Domain.Menu;
using MUEats.Restaurants.Core.Domain.Menu.Entities;
using MUEats.Restaurants.Core.Domain.Menu.Entities.ValueObjects;
using MUEats.Restaurants.Core.Domain.Restaurant;
using MUEats.Restaurants.Infrastructure.ExternalServices.Api;
using MUEats.Restaurants.Infrastructure.Persistence.Inbox;
using MUEats.Restaurants.Infrastructure.Persistence.Outbox;

namespace MUEats.Restaurants.Infrastructure.Persistence.Contexts;

public class RestaurantsDbContext : DbContext
{
    public RestaurantsDbContext(DbContextOptions<RestaurantsDbContext> options) :  base(options){}
    
    public DbSet<Menu> Menus { get; set; }
    
    public DbSet<Restaurant> Restaurants { get; set; }
    
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    
    public DbSet<InboxMessage>  InboxMessages { get; set; }
    
    public DbSet<OrderSnapshot> OrderSnapshots { get; set; }
    
    public DbSet<OrderItemSnapshot> OrderItemSnapshots { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RestaurantsDbContext).Assembly);
    }
}