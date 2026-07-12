using Ardalis.Specification;
using MUEats.Discounts.Core.Discount;

namespace MUEats.Discounts.Application.Ports;

public interface IDiscountsRepository
{
    Task AddAsync(Discount discount, CancellationToken ct);
    
    Task<Discount?> GetByIdAsync(Guid discountId, CancellationToken ct);

    Task<List<Discount>> ListAsync(ISpecification<Discount> specification, CancellationToken ct);

    Task DeleteAsync(Discount discount, CancellationToken ct);
}