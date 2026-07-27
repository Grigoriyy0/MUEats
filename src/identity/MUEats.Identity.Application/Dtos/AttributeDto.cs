namespace MUEats.Identity.Application.Dtos;

public sealed record AttributeDto
{
    public string Key { get; init; }
    
    public string Value { get; init; }
}