using ApiContracts.Extensions.Exceptions;
using ApiContracts.Tests.Helpers;
using ApiContracts.Validators;
using System.Text.Json;

namespace ApiContracts.Tests;

public class ContractValidatorTests
{
    private readonly ContractValidator _sut;
    public ContractValidatorTests()
    {
        _sut = new ContractValidator();
        
    }
    [Fact]
    public void GivenValidate_WhenModelPropertiesHasNone_ThenThrowContractValidatonFailedException()
    {
        // Arrange
        var model = new SampleModelWithoutAcceptance();
        var body = JsonDocument.Parse("{}").RootElement;
        var contractName = "SampleContract";

        // Act & Assert
        Assert.Throws<ContractValidationFailedException>(() => _sut.Validate(model, body, contractName));
    }

    [Fact]
    public void GivenValidate_WhenMissingProperties_ThenThrowContractValidatonFailedException()
    {
        // Arrange
        var model = new SampleModelWithAcceptance();
        var body = JsonDocument.Parse("{\"name\":\"John\"}").RootElement;
        var contractName = "SampleContract";

        // Act & Assert
        Assert.Throws<ContractValidationFailedException>(() => _sut.Validate(model, body, contractName));
    }

    [Fact]
    public void GivenValidate_WhenBodyPropertiesHasExtra_ThenThrowContractValidatonFailedException()
    {
        // Arrange
        var model = new SampleModelWithAcceptance();
        var body = JsonDocument.Parse("{\"name\":\"John\",\"age\":30,\"extra\":\"extra\"}").RootElement;
        var contractName = "SampleContract";

        // Act & Assert
        Assert.Throws<ContractValidationFailedException>(() => _sut.Validate(model, body, contractName));
    }

    [Fact]
    public void GivenValidate_WhenBodyPropertiesHasMissing_ThenThrowContractValidatonFailedException()
    {
        // Arrange
        var model = new SampleModelWithAcceptance();
        var body = JsonDocument.Parse("{\"age\":30}").RootElement;
        var contractName = "SampleContract";

        // Act & Assert
        Assert.Throws<ContractValidationFailedException>(() => _sut.Validate(model, body, contractName));
    }

    [Fact]
    public void GivenValidate_WhenBodyPropertiesHasNull_ThenThrowContractValidatonFailedException()
    {
        // Arrange
        var model = new SampleModelWithAcceptance();
        var body = JsonDocument.Parse("{\"name\":null,\"age\":30}").RootElement;
        var contractName = "SampleContract";

        // Act & Assert
        Assert.Throws<ContractValidationFailedException>(() => _sut.Validate(model, body, contractName));
    }

    [Fact]
    public void GivenValidate_WhenBodyPropertiesHasArray_ThenThrowContractValidatonFailedException()
    {
        // Arrange
        var model = new SampleModelWithAcceptance();
        var body = JsonDocument.Parse("{\"name\":\"John\",\"age\":30,\"extra\":[{\"name\":\"John\",\"age\":30}]}").RootElement;
        var contractName = "SampleContract";

        // Act & Assert
        Assert.Throws<ContractValidationFailedException>(() => _sut.Validate(model, body, contractName));
    }

    [Fact]
    public void GivenValidate_WhenBodyPropertiesHasNestedObject_ThenThrowContractValidatonFailedException()
    {
        // Arrange
        var model = new SampleModelWithAcceptance();
        var body = JsonDocument.Parse("{\"name\":\"John\",\"age\":30,\"extra\":{\"name\":\"John\",\"age\":30}}").RootElement;
        var contractName = "SampleContract";

        // Act & Assert
        Assert.Throws<ContractValidationFailedException>(() => _sut.Validate(model, body, contractName));
    }

    [Fact]
    public void GivenValidate_WhenBodyPropertiesHasNestedArray_ThenThrowContractValidatonFailedException()
    {
        // Arrange
        var model = new SampleModelWithAcceptance();
        var body = JsonDocument.Parse("{\"name\":\"John\",\"age\":30,\"extra\":[{\"name\":\"John\",\"age\":30}]}").RootElement;
        var contractName = "SampleContract";

        // Act & Assert
        Assert.Throws<ContractValidationFailedException>(() => _sut.Validate(model, body, contractName));
    }
}
