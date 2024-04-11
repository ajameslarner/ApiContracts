using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using ApiContracts.Extensions.Attributes;
using System.Reflection;
using Microsoft.AspNetCore.Http.Metadata;
using ApiContracts.Models.Abstract;
using ApiContracts.Extensions;
using ApiContracts.Constants;

namespace ApiContracts.Middleware;

public class ContractMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        var model = GetContractBoundModel(context);

        if (model is null)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(Headers.ContractName, out StringValues contractName))
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync($"Missing '{Headers.ContractName}' header value. The model on this endpoint is under contract.");
            return;
        }

        var body = await DeserializeRequestBody(context);

        try 
        {
            ValidateContract(model, body, contractName);
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(ex.Message);
        }
    }

    // TODO: Refactor this
    private static void ValidateContract(object? model, JsonElement body, string? service)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));
        ArgumentNullException.ThrowIfNull(service, nameof(service));

        var properties = model.GetType().GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(AcceptanceAttribute<>)));

        var errors = new List<string>();

        foreach (var property in properties)
        {
            var attributes = property.GetCustomAttributes(typeof(AcceptanceAttribute<>));

            if (!attributes.Any())
                errors.Add($"No 'AcceptanceAttribute' found for property '{property.Name}'");

            if (body.TryGetProperty(property.Name.ToCamelCase(), out var _) && !attributes.Any(attr => (attr.GetType().GetProperty("Contract")?.GetValue(attr) as Contract)?.Service == service))
            {
                errors.Add($"The property '{property.Name}' is not included as part in acceptance of the contract: '{service}'");
                continue;
            }

            foreach (var attribute in attributes)
            {
                var contractProperty = attribute.GetType().GetProperty("Contract");
                var contract = contractProperty?.GetValue(attribute) as Contract;

                if (contract?.Service != service)
                    continue;

                if (!IsCamelCase(body))
                {
                    errors.Add("Request body must be in camelCase");
                    continue;
                }

                if (!body.TryGetProperty(property.Name.ToCamelCase(), out var propertyValue))
                {
                    errors.Add($"Property '{property.Name.ToCamelCase()}' not found in request body to fulfill the contract: '{contract.Service}'");
                    continue;
                }

                var requiredProperty = attribute.GetType().GetProperty("Required");
                var required = requiredProperty?.GetValue(attribute) as bool?;

                if (required == true && (propertyValue.ValueKind == JsonValueKind.Null || propertyValue.ValueKind == JsonValueKind.Undefined))
                {
                    errors.Add($"Property value for '{property.Name.ToCamelCase()}' is required to fulfill the contract: '{contract.Service}'");
                    continue;
                }
            }
        }

        // Check if there are any extra properties in the request body that do not match with the model
        foreach (var element in body.EnumerateObject())
        {
            var propertyName = element.Name.ToPascalCase();
            if (!properties.Any(p => p.Name == propertyName))
            {
                errors.Add($"Extra property '{element.Name}' found in request body that does not match with the '{service}' contract in the model.");
            }
        }

        if (errors.Any())
            throw new ArgumentException(string.Join(Environment.NewLine, errors));
    }

    // TODO: Need to handle xml and other content types
    private static async Task<JsonElement> DeserializeRequestBody(HttpContext context)
    {
        context.Request.EnableBuffering();

        if (!context.Request.Body.CanRead)
            return default;


        var body = string.Empty;

        using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
        {
            body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        if (string.IsNullOrEmpty(body))
            return default;

        return JsonDocument.Parse(body).RootElement;
    }

    private static bool IsCamelCase(JsonElement jsonElement)
    {
        foreach (var property in jsonElement.EnumerateObject())
        {
            string propertyName = property.Name;
            string camelCasePropertyName = propertyName.ToCamelCase();

            if (propertyName != camelCasePropertyName)
            {
                return false;
            }
        }

        return true;
    }

    private static object? GetContractBoundModel(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint is null)
            return null;

        var acceptsMetadata = endpoint.Metadata.GetMetadata<IAcceptsMetadata>();
        if (acceptsMetadata is null)
            return null;

        var requestType = acceptsMetadata.RequestType;
        if (requestType is null)
            return null;

        if (requestType.GetCustomAttributes(typeof(ContractBoundAttribute), true).Length <= 0)
            return null;

        return Activator.CreateInstance(requestType);
    }
}
