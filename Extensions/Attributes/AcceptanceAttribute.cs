using ApiContracts.Models.Abstract;

namespace ApiContracts.Extensions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class AcceptanceAttribute<T> : Attribute where T : Contract, new()
{
    public T Contract => new();
    public bool Required { get; set; }

    public AcceptanceAttribute() { }
}