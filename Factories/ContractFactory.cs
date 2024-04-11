//using ApiContracts.Core;
//using ApiContracts.Factories.Abstract;
//using System.Reflection;

//namespace ApiContracts.Factories;

//public class ContractFactory(IServiceProvider serviceProvider) : IContractFactory
//{
//    private readonly Dictionary<string, Type> _contracts = Assembly.GetExecutingAssembly()
//        .GetTypes()
//        .Where(t => t.GetCustomAttributes(typeof(OfferAttribute)) != null)
//        .ToDictionary(t => (Activator.CreateInstance(t) as Contract).Service, t => t);

//    public Contract? Create(string service)
//    {
//        if (!_contracts.TryGetValue(service, out var type))
//            throw new ArgumentException($"Invalid type name: {service}");

//        return serviceProvider.GetService(type) as Contract;
//    }
//}