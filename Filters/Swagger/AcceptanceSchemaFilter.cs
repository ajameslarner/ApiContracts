using ApiContracts.Extensions;
using ApiContracts.Extensions.Attributes;
using ApiContracts.Models.Abstract;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace ApiContracts.Filters.Swagger;

public class AcceptanceSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        if (type.GetCustomAttributes(typeof(ContractBoundAttribute), true).Length <= 0)
            return;

        schema.Description += "ContractBound";

        foreach (var property in type.GetProperties())
        {
            var attributes = property.GetCustomAttributes(typeof(AcceptanceAttribute<>));

            if (!schema.Properties.TryGetValue(property.Name.ToCamelCase(), out var propertySchema))
                continue;

            List<string> contracts = [];
            foreach (var attribute in attributes)
            {
                var contract = attribute?.GetType()?.GetProperty("Contract")?.GetValue(attribute) as Contract;

                if (contract != null)
                    contracts.Add(contract.Name);
            }
            propertySchema.Description = "Contracts: " + string.Join(", ", contracts);
        }
    }
}
