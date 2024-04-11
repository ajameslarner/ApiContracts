using ApiContracts.Middleware;
using Microsoft.AspNetCore.Builder;

namespace ApiContracts.Extensions;
public static class DependencyInjection
{
    public static IApplicationBuilder UseApiContracts(this IApplicationBuilder app) => app.UseMiddleware<ContractMiddleware>();

    //public static IServiceCollection AddApiContracts(this IServiceCollection services)
    //{
    //    var offerTypes = Assembly.GetExecutingAssembly().GetTypes()
    //        .Where(t => t.GetCustomAttributes(typeof(OfferAttribute)) != null);

    //    foreach (var type in offerTypes)
    //    {
    //        services.AddSingleton(type);
    //    }

    //    return services;
    //}
}