namespace MUEats.Identity.Application.Ports;

public interface IHashProvider
{
    string ComputeHash(string input);

    bool VerifyHash(string password, string hash);
}