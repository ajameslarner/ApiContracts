namespace ApiContracts.Models.Abstract;

/// <summary>
/// The contract class that defines the acceptance criteria for the model.
/// </summary>
public abstract class Contract
{
    /// <summary>
    /// The name of the contract.
    /// </summary>
    public abstract string Name { get; }
}