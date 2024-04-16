using ApiContracts.Middleware;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace ApiContracts.Tests;

public class ContractMiddlewareTests
{
    private readonly RequestDelegate _next = Substitute.For<RequestDelegate>();
    private readonly ContractMiddleware _sut;

    public ContractMiddlewareTests()
    {
        _sut = new ContractMiddleware(_next);
    }

    [Fact]
    public async Task GivenInvoke_WhenModelIsNull_ThenCallNext()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{}"));

        // Act
        await _sut.Invoke(context);

        // Assert
        await _next.Received(1).Invoke(context);
    }
}