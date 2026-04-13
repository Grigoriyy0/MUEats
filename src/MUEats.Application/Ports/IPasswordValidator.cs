namespace MUEats.Application.Ports;

public interface IPasswordValidator
{
    bool Validate(string password);
}