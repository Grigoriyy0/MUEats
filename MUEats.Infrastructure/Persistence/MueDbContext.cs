using Microsoft.EntityFrameworkCore;
using MUEats.Core;
using MUEats.Core.Domain.Order;
using MUEats.Core.Domain.Order.Entities;
using MUEats.Core.Domain.Order.ValueObjects;
using MUEats.Core.Domain.Restaurant;
using MUEats.Core.Domain.Restaurant.Entities;
using MUEats.Core.Domain.ShoppingCart;
using MUEats.Core.Domain.ShoppingCart.ValueObjects;
using MUEats.Core.Domain.User;
using MUEats.Core.Domain.User.Entities;

namespace MUEats.Infrastructure.Persistence;

public class MueDbContext : DbContext
{
    public MueDbContext(DbContextOptions<MueDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<OrderItem> OrderItems { get; set; }
    
    public DbSet<Restaurant> Restaurants { get; set; }
    
    public DbSet<FoodItem>  FoodItems { get; set; }
    
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    
    public DbSet<CartItem> CartItems { get; set; }
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    
    public DbSet<OrderSagaState>  OrderSagaStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MueDbContext).Assembly);
    }
}