namespace MUEats.Application.Ports;

public interface ICurrentUserContext
{
    Guid GetUserId();
    
    bool IsAuthenticated();
}