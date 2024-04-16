using ApiContracts.Models.Abstract;

namespace ApiContracts.Extensions.Attributes;

/// <summary>
/// The acceptance attribute class that defines the acceptance criteria for the model.
/// </summary>
/// <typeparam name="T">The type of the contract defined</typeparam>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class AcceptanceAttribute<T> : Attribute where T : Contract, new()
{
    /// <summary>
    /// The contract defined by the acceptance attribute.
    /// </summary>
    public T Contract => new();
    /// <summary>
    /// The required flag for the acceptance attribute.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// The acceptance attribute constructor.
    /// </summary>
    public AcceptanceAttribute() { }
}