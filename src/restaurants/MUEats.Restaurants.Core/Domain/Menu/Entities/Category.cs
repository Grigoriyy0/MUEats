using CSharpFunctionalExtensions;
using Primitives;

namespace MUEats.Restaurants.Core.Domain.Menu.Entities;

public class Category
{
    private Category(string name, string? description)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
    }
    
    public Guid Id { get; init; }
    
    public string Name { get; private set; }
    
    public string? Description { get; private set; }

    public static Result<Category, Error> Create(string name, string? description)
    {
        if (string.IsNullOrEmpty(name))
        {
            return DomainErrors.MenuCategory.CategoryNameIsEmpty;
        }
        
        return new Category(name, description);
    }
}