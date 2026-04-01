namespace MUEats.Application.Ports;

public interface ICourierProvider
{
    Task<Guid> RequestAsync(string orderDetails);

    Task CancelAsync(Guid courierId);
}