using ApiContracts.Constants;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiContracts.Filters.Swagger;

/// <summary>
/// The contract header filter class that adds the contract header to the swagger documentation.
/// </summary>
public class ContractHeaderFilter : IOperationFilter
{
    /// <summary>
    /// Applies the contract header to the operation.
    /// </summary>
    /// <param name="operation">The current ApiOperation object</param>
    /// <param name="context">The current context object</param>
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