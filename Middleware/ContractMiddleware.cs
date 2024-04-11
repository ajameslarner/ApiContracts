using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using ApiContracts.Extensions.Attributes;
using System.Reflection;
using ApiContracts.Core;
using Microsoft.AspNetCore.Http.Metadata;

namespace ApiContracts.Middleware;

public class ContractMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        if (!IsContractBound(context))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("Contract-Service", out StringValues service))
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("Missing service name. This endpoint is under contract.");
            return;
        }

        var model = GetEndpointModel(context);

        if (model is null)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("Invalid service name. This endpoint is under contract.");
            return;
        }

        var body = await DeserializeRequestBody(context);

        try 
        {
            ValidateContract(model, body, service);
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(ex.Message);
        }
    }

    private static void ValidateContract(object? model, JsonElement body, string? service)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));
        ArgumentNullException.ThrowIfNull(service, nameof(service));

        var properties = model.GetType().GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(AcceptanceAttribute<>)));

        foreach (var property in properties)
        {
            var attributes = property.GetCustomAttributes().Where(a => a.GetType().IsGenericType && a.GetType().GetGenericTypeDefinition() == typeof(AcceptanceAttribute<>));

            if (!attributes.Any())
                throw new ArgumentException($"No AcceptanceAttribute found for property {property.Name}");

            foreach (var attribute in attributes)
            {
                var contractProperty = attribute.GetType().GetProperty("Contract");
                var contract = contractProperty?.GetValue(attribute) as Contract;

                if (contract?.Service != service)
                    continue;

                if (!IsCamelCase(body))
                    throw new ArgumentException("Request body must be in camelCase");

                if (!body.TryGetProperty(char.ToLowerInvariant(property.Name[0]) + property.Name[1..], out var propertyValue))
                    throw new ArgumentException($"Property {property.Name[0] + property.Name[1..]} not found in request body to validate against: {contract.Service}");

                var requiredProperty = attribute.GetType().GetProperty("Required");
                var required = requiredProperty?.GetValue(attribute) as bool?;

                if (required == true && (propertyValue.ValueKind == JsonValueKind.Null || propertyValue.ValueKind == JsonValueKind.Undefined))
                    throw new ArgumentException($"Property {property.Name[0] + property.Name[1..]} is required as defined by the {contract.Service} contract.");
            }
        }
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
            string camelCasePropertyName = char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);

            if (propertyName != camelCasePropertyName)
            {
                return false;
            }
        }

        return true;
    }

    private static object? GetEndpointModel(HttpContext context)
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

        if (requestType.GetCustomAttributes(typeof(OfferAttribute), true).Length <= 0)
            return null;

        return Activator.CreateInstance(requestType);
    }

    private static bool IsContractBound(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var hasContractBoundAttribute = endpoint?.Metadata.GetMetadata<ContractBoundAttribute>() != null;
        return hasContractBoundAttribute;
    }
}
