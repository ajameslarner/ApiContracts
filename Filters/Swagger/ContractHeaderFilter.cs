using ApiContracts.Constants;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiContracts.Filters.Swagger;

public class ContractHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = Headers.ContractName,
            In = ParameterLocation.Header,
            Required = false, // TODO: When its required it doesn't accept any values in the UI just fails validation
            Schema = new OpenApiSchema
            {
                Type = "String"
            }
        });
    }
}