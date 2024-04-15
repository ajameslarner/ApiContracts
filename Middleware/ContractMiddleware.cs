using Microsoft.AspNetCore.Http;
using System.Text.Json;
using ApiContracts.Extensions.Attributes;
using Microsoft.AspNetCore.Http.Metadata;
using ApiContracts.Extensions;
using ApiContracts.Constants;
using ApiContracts.Validators;
using ApiContracts.Extensions.Exceptions;

namespace ApiContracts.Middleware;

public class ContractMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    private readonly ContractValidator _contractValidator = new();

    public async Task Invoke(HttpContext context)
    {
        var model = GetContractBoundModel(context);

        if (model is null)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(Headers.ContractName, out var contractName))
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync($"Missing '{Headers.ContractName}' header value. The model on this endpoint is under contract.");
            return;
        }

        var body = await DeserializeRequestBody(context);

        try
        {
            _contractValidator.Validate(model, body, contractName);
            await _next(context);
        }
        catch (ContractValidationFailedException ex)
        {
            context.Response.StatusCode = ex.ErrorCode;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(ex.Message);
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
