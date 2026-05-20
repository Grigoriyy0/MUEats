namespace Primitives;

public class GeneralError
{
    public static Error ValueIsIncorrect(string valueName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valueName);
        
        return Error.Create(valueName, $"{valueName} is incorrect");
    }
}