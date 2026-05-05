using Microsoft.OpenApi;

namespace MUEats.Restaurants.Api.Extensions;

public static class SwaggerExtensions
{
    public static void AddSwaggerExtensions(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            const string securitySchemeId = "bearer"; 
    
            options.AddSecurityDefinition(securitySchemeId, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",          
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme. Enter your token below."
            });
    
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(securitySchemeId, document)] = []
            });
        });   
    }
}