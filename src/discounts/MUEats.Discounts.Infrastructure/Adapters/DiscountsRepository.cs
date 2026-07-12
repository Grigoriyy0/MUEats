using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MUEats.Discounts.Application.Ports;
using MUEats.Discounts.Core.Discount;
using MUEats.Discounts.Infrastructure.Persistence.Contexts;

namespace MUEats.Discounts.Infrastructure.Adapters;

public class DiscountsRepository : IDiscountsRepository
{
    private readonly DiscountsDbContext _context;

    public DiscountsRepository(DiscountsDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Discount discount, CancellationToken ct)
    {
        return _context.Discounts.AddAsync(discount, ct)
            .AsTask();
    }

    public Task<Discount?> GetByIdAsync(Guid discountId, CancellationToken ct)
    {
        return _context.Discounts.FirstOrDefaultAsync(x => x.Id == discountId, ct);
    }

    public Task<List<Discount>> ListAsync(ISpecification<Discount> specification, CancellationToken ct)
    {
        return _context.Discounts.WithSpecification(specification)
            .AsNoTracking()
            .ToListAsync(ct);
    }
    
    public Task DeleteAsync(Discount discount, CancellationToken ct)
    {
        _context.Remove(discount);
        return Task.CompletedTask;
    }
}