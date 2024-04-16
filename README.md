# ApiContracts

[![Package CI & CD](https://github.com/ajameslarner/ApiContracts/actions/workflows/package_ci_cd.yml/badge.svg)](https://github.com/ajameslarner/ApiContracts/actions/workflows/package_ci_cd.yml)
[![Build Version](https://github.com/ajameslarner/ApiContracts/actions/workflows/build_version.yml/badge.svg?branch=main)](https://github.com/ajameslarner/ApiContracts/actions/workflows/build_version.yml)


### What is ApiContracts?

ApiContracts is a tool designed to help developers create contract specifications on the entity models of their API endpoints. This allows for validation of incoming request DTOs against those contracts, ensuring that only DTOs containing all the data/properties as defined by the contract are accepted.

## Features

- **Contract Creation**: Define contract specifications for your entity models with ease.
- **Request Validation**: Validate incoming request DTOs against the defined contracts.
- **Data Integrity**: Ensure that all incoming DTOs contain the necessary data/properties.

## How do I get started?

### Server

1. First, define your contracts by inheriting from the `Contract` abstract class

```csharp
namespace SampleAPI.Contracts;

public class BasicUserContract : Contract
{
    public override string Service => "BasicUserContract";
}

public class FullUserContract : Contract
{
    public override string Service => "FullUserContract";
}
```

2. Then, use the `ContractBound` attribute to define your entity model under contract

```csharp
namespace SampleAPI.Models;

[ContractBound]
public class SampleModel
{

}
```

3. Provide the acceptance criteria for your contract specifications using the `Acceptance<T>` attribute, declaring if the value is required or optional by default.

```csharp
namespace SampleAPI.Models;

[ContractBound]
public class SampleModel
{
    [Acceptance<BasicUserContract>(Required = true)]
    [Acceptance<FullUserContract>(Required = true)]
    public string? Name { get; set; }

    [Acceptance<BasicUserContract>(Required = true)]
    [Acceptance<FullUserContract>(Required = true)]
    public int Age { get; set; }

    [Acceptance<BasicUserContract>(Required = true)]
    [Acceptance<FullUserContract>(Required = true)]
    public string? Email { get; set; }

    [Acceptance<FullUserContract>()]
    public string? PhoneNumber { get; set; }

    [Acceptance<FullUserContract>(Required = true)]
    public string? Address { get; set; }

    [Acceptance<FullUserContract>()]
    public string? City { get; set; }

    [Acceptance<FullUserContract>(Required = true)]
    public string? State { get; set; }
}
```

4. Register ApiContracts on your web API configuration pipeline using the `UseApiContracts()` extension method.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test API", Version = "v1" });
});

builder.Services.AddApiContractDocs();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseApiContracts(); // Register ApiContracts
app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();
```

5. Optionally, you can assign the Swagger/OpenAPI documentation like supported

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test API", Version = "v1" });
});

builder.Services.AddApiContractDocs(); // Add ApiContractDocs

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseApiContracts();
app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();
```

6. Assign the `SampleModel` as your API entity model (here's a minimal API example)

```csharp
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
```

### Client

Once you have setup your API with a `ContractBound` entity model, the endpoint will be 'under-contract'. This means you need to provide a service name on the custom `Contract-Name` header with your request.

TypeScript Sample Client

```typescript
interface SampleModel {
    // Define the properties of SampleModel here
}

async function callTestEndpoint(model: SampleModel) {
    const response = await fetch('https://your-api-url/test', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Contract-Name': 'FullUserContract' // Define your target contract here
        },
        body: JSON.stringify(model)
    });

    if (!response.ok) {
        const message = `An error has occurred: ${response.status}`;
        throw new Error(message);
    }

    const data: SampleModel = await response.json();
    return data;
}

// Usage:
const model: SampleModel = {
    // Populate the model properties here
};

callTestEndpoint(model)
    .then(data => console.log(data))
    .catch(error => console.error(error));

```

C# Sample Client

```csharp
public static async Task CallTestEndpoint(User model)
{
    var json = JsonConvert.SerializeObject(model);
    var data = new StringContent(json, Encoding.UTF8, "application/json");

    client.DefaultRequestHeaders.Add("Contract-Name", "FullUserContract"); // Define your target contract here

    var response = await client.PostAsync("https://your-api-url/test", data);

    string result = response.Content.ReadAsStringAsync().Result;
    Console.WriteLine(result);
}
```

### Where can I get it?

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [ApiContracts](https://www.nuget.org/packages/ApiContracts/) from the package manager console:

```
PM> Install-Package ApiContracts
```
Or from the .NET CLI as:
```
dotnet add package ApiContracts
```

### Do you have an issue?

If you're still running into problems, please don't hesitate to file an issue in this repository. We appreciate your feedback and contributions!

### License

ApiContracts Copyright © 2024 Anthony Larner under the MIT license.
