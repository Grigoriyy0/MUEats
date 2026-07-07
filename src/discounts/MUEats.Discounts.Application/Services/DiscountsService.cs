using MUEats.Discounts.Application.Dtos;
using MUEats.Discounts.Application.Ports;

namespace MUEats.Discounts.Application.Services;

public class DiscountsService
{
    private readonly IUnitOfWork _uow;
    private readonly IDiscountsRepository _discounts;

    public DiscountsService(IUnitOfWork uow, IDiscountsRepository discounts)
    {
        _uow = uow;
        _discounts = discounts;
    }

    public async Task CreateAsync(CreateDiscountDto dto, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
    }
}