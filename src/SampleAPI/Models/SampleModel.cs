using ApiContracts.Extensions.Attributes;
using SampleAPI.Contracts;

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

    [Acceptance<FullUserContract>(Required = true)]
    public NestedSampleModel ComplexProperty { get; set; }
}

public class NestedSampleModel
{
    [Acceptance<FullUserContract>(Required = true)]
    public string? NestedProperty { get; set; }
}