using ApiContracts.Core;
using ApiContracts.Extensions.Attributes;

namespace ApiContracts;

public class AccidentReportContract : Contract
{
    public override string Service => "AccidentReport";
}

class NearMissContract : Contract
{
    public override string Service => "NearMissReport";
}

class HazardContract : Contract
{
    public override string Service => "HazardReport";
}

[Offer]
public class SafetyReportModel
{
    [Acceptance<AccidentReportContract>(Required = true)]
    [Acceptance<NearMissContract>(Required = true)]
    [Acceptance<HazardContract>(Required = true)]
    public string SessionId { get; set; }
}