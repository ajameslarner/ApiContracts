using ApiContracts.Core;

namespace ApiContracts.Factories.Abstract;

public interface IContractFactory
{
    Contract? Create(string key);
}