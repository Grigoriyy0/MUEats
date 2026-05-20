using CSharpFunctionalExtensions;
using Primitives;

namespace MUEats.Restaurants.Core.Domain.Menu.Entities;

public class Category
{
    private Category(string name, 
        string? description,
        Guid menuId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        MenuId = menuId;
    }
    
    public Guid Id { get; init; }
    
    public Guid MenuId { get; private set; }
    
    public string Name { get; private set; }
    
    public string? Description { get; private set; }

    public static Result<Category, Error> Create(string name, 
        string? description,
        Guid menuId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return DomainErrors.MenuCategory.CategoryNameIsEmpty;
        }
        
        return new Category(name, description, menuId);
    }
}