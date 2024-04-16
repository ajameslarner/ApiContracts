using Microsoft.AspNetCore.Mvc;
using SampleAPI.Models;

namespace SampleAPI.Extensions;

public static class Endpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/test", ([FromBody] SampleModel model) =>
        {
            if (model is null)
                return Results.BadRequest("Invalid model.");

            return Results.Ok(model);
        })
        .WithName("GetTest")
        .WithOpenApi()
        .Produces(StatusCodes.Status400BadRequest)
        .Produces<SampleModel>(StatusCodes.Status200OK);
    }
}
