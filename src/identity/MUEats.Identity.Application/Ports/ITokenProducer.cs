using MUEats.Identity.Core.Domain.Role;
using MUEats.Identity.Core.Domain.User;

namespace MUEats.Identity.Application.Ports;

public interface ITokenProducer
{
    string ProduceToken(User user, List<Role> roles);
}