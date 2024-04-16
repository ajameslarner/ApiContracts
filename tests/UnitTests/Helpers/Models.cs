using ApiContracts.Extensions.Attributes;
using ApiContracts.Models.Abstract;

namespace ApiContracts.Tests.Helpers;
public class SampleContract : Contract
{
    public override string Name => "SampleContract";
}

[ContractBound]
public class SampleModelWithAcceptance
{
    [Acceptance<SampleContract>(Required = true)]
    public string Name { get; set; }

    [Acceptance<SampleContract>(Required = true)]
    public int Age { get; set; }

    [Acceptance<SampleContract>]
    public SampleModelWithAcceptance[]? SampleNestedPropertyArray { get; set; }

    [Acceptance<SampleContract>]
    public SampleModelWithAcceptance? SampleNestedProperty { get; set; }
}

public class SampleModelWithoutAcceptance
{
    public string Name { get; set; }

    public int Age { get; set; }
}