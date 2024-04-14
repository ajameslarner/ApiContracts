using ApiContracts.Extensions;
using ApiContracts.Extensions.Attributes;
using ApiContracts.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace ApiContracts.Validators
{
    public class ContractValidator
    {
        private Dictionary<string, PropertyInfo> _cachedModelProperties;
        private Dictionary<string, JsonProperty> _cachedBodyProperties;
        private string _contractName;

        private void Setup(object model, JsonElement body, string contractName)
        {
            _cachedModelProperties = model.GetType().GetProperties()
                .Where(prop => prop != null && Attribute.IsDefined(prop, typeof(AcceptanceAttribute<>)))
                .ToDictionary(prop => prop.Name.ToCamelCase(), prop => prop);

            _cachedBodyProperties = body.EnumerateObject()
                .ToDictionary(prop => prop.Name, prop => prop);

            _contractName = contractName;
        }

        public void Validate(object model, JsonElement body, string contractName)
        {
            Setup(model, body, contractName);

            foreach (var bodyProp in _cachedBodyProperties)
            {
                if (!_cachedModelProperties.TryGetValue(bodyProp.Key, out PropertyInfo? modelProp))
                    throw new Exception($"Extra property '{bodyProp.Key}' found in request body that is not part of acceptance on the contract: '{_contractName}'");

                var attributes = modelProp.GetCustomAttributes(typeof(AcceptanceAttribute<>));

                foreach (var attribute in attributes)
                {
                    var contractProperty = attribute.GetType().GetProperty("Contract");
                    var contract = contractProperty?.GetValue(attribute) as Contract;

                    if (contract?.Service != _contractName)
                        continue;

                    var requiredProperty = attribute.GetType().GetProperty("Required");
                    var required = requiredProperty?.GetValue(attribute) as bool?;

                    if (required == true && bodyProp.Value.Value.ValueKind == JsonValueKind.Null)
                        throw new Exception($"Property value for '{modelProp.Name.ToCamelCase()}' is required to fulfill the contract: '{contract.Service}'");

                    if (bodyProp.Value.Value.ValueKind == JsonValueKind.Object)
                        Validate(Activator.CreateInstance(modelProp.PropertyType), bodyProp.Value.Value, contract.Service); // TODO: Get value from bodyProp.Value.Value to pass to recursive call
                }
            }
        }
    }
}