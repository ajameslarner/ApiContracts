using ApiContracts.Filters.Swagger;
using ApiContracts.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ApiContracts.Extensions;

/// <summary>
/// The dependency injection class that adds the api contract middleware and swagger documentation.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// The method that adds the api contract middleware to the application.
    /// </summary>
    /// <param name="app">The app builder object</param>
    /// <returns>The app builder object</returns>
    public static IApplicationBuilder UseApiContracts(this IApplicationBuilder app) => app.UseMiddleware<ContractMiddleware>();

    /// <summary>
    /// The method that adds the api contract documentation to the services.
    /// </summary>
    /// <param name="services">The service collection object</param>
    /// <returns>The service collection object</returns>
    public static IServiceCollection AddApiContractDocs(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<ContractHeaderFilter>();
            c.SchemaFilter<AcceptanceSchemaFilter>();
        });

        return services;
    }
}