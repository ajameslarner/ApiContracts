using ApiContracts.Extensions;
using ApiContracts.Extensions.Attributes;
using ApiContracts.Extensions.Exceptions;
using ApiContracts.Models.Abstract;
using System.Reflection;
using System.Text.Json;

namespace ApiContracts.Validators
{
    public class ContractValidator
    {
        private Dictionary<string, PropertyInfo> _cachedModelProperties = [];
        private Dictionary<string, JsonProperty> _cachedBodyProperties = [];
        private IEnumerable<string> _cachedMissingProperties = [];
        private IEnumerable<string> _cachedExtraProperties = [];

        private void CacheProperties(object model, JsonElement body, string contractName)
        {
            _cachedModelProperties = model.GetType().GetProperties()
                .Where(prop => prop != null && prop.GetCustomAttributes(false)
                .Any(attr => attr.GetType().IsGenericType &&
                     attr.GetType().GetGenericTypeDefinition() == typeof(AcceptanceAttribute<>) &&
                     (attr as dynamic).Contract.Name == contractName))
                .ToDictionary(prop => prop.Name.ToCamelCase(), prop => prop);

            _cachedBodyProperties = body.EnumerateObject()
                .ToDictionary(prop => prop.Name.ToCamelCase(), prop => prop);

            _cachedMissingProperties = _cachedModelProperties.Keys.Except(_cachedBodyProperties.Keys);

            _cachedExtraProperties = _cachedBodyProperties.Keys.Except(_cachedModelProperties.Keys);
        }

        //TODO - Validate exclusively against the contract, basic contract is valid with all properties present, should be invalid
        public void Validate(object model, JsonElement body, string contractName)
        {
            CacheProperties(model, body, contractName);

            if (_cachedModelProperties.Count == 0)
                throw new ContractValidationFailedException(400, $"No properties found on the model that match the contract: '{contractName}'");

            if (_cachedMissingProperties.Any())
                throw new ContractValidationFailedException(400, $"The following properties are missing from the request body: {string.Join(", ", _cachedMissingProperties)}");
            
            if (_cachedExtraProperties.Any())
                throw new ContractValidationFailedException(400, $"The following properties are not part of the acceptance criteria for '{contractName}': {string.Join(", ", _cachedExtraProperties)}");

            foreach (var modelProperty in _cachedModelProperties)
            {
                if (!_cachedBodyProperties.TryGetValue(modelProperty.Key, out JsonProperty bodyProperty))
                    throw new ContractValidationFailedException(400, $"The property '{modelProperty.Key}' is missing from the request body as declared in the acceptance criteria on the contract: '{contractName}'");

                var attribute = modelProperty.Value.GetCustomAttributes(typeof(AcceptanceAttribute<>))
                    .FirstOrDefault(attr => attr?.GetType()?.GetProperty("Required")?.GetValue(attr) as bool? == true);

                if (attribute != null && bodyProperty.Value.ValueKind == JsonValueKind.Null)
                    throw new ContractValidationFailedException(400, $"Property value for '{bodyProperty.Name.ToCamelCase()}' is required to fulfill the contract: '{contractName}'");

                if (bodyProperty.Value.ValueKind == JsonValueKind.Array)
                {
                    var jsonArray = bodyProperty.Value.GetRawText();
                    var nestedArray = JsonDocument.Parse(jsonArray).RootElement;

                    foreach (var item in nestedArray.EnumerateArray())
                    {
                        if (item.ValueKind != JsonValueKind.Object)
                            continue;

                        var modelObject = Activator.CreateInstance(modelProperty.Value.PropertyType)
                            ?? throw new ContractValidationFailedException(500, $"Failed to create instance of '{modelProperty.Value.PropertyType.Name}' for property '{modelProperty.Value.Name}'");

                        Validate(modelObject, item, contractName);
                    }
                }

                if (bodyProperty.Value.ValueKind == JsonValueKind.Object)
                {
                    var jsonObject = bodyProperty.Value.GetRawText();
                    var nestedObject = JsonDocument.Parse(jsonObject).RootElement;

                    var modelObject = Activator.CreateInstance(modelProperty.Value.PropertyType) 
                        ?? throw new ContractValidationFailedException(500, $"Failed to create instance of '{modelProperty.Value.PropertyType.Name}' for property '{modelProperty.Value.Name}'");
                        
                    Validate(modelObject, nestedObject, contractName);
                }
            }
        }
    }
}