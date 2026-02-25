using MUEats.Application.Responses;
using MUEats.Core.Domain.User;

namespace MUEats.Application.Ports;

public interface ITokenProducer
{
    string ProduceToken(User user);

    string ProduceRefreshToken();
    
    TokenResponse ProduceTokenPair(User user);
}