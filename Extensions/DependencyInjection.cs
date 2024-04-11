using ApiContracts.Filters.Swagger;
using ApiContracts.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ApiContracts.Extensions;
public static class DependencyInjection
{
    public static IApplicationBuilder UseApiContracts(this IApplicationBuilder app) => app.UseMiddleware<ContractMiddleware>();

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