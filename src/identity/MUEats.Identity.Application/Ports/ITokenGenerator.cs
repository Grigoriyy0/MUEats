using MUEats.Identity.Core.Domain.Role;
using MUEats.Identity.Core.Domain.User;

namespace MUEats.Identity.Application.Ports;

public interface ITokenGenerator
{
    string ProduceToken(User user, List<Role> roles);
}