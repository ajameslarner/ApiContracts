namespace ApiContracts.Extensions.Exceptions;
public class ContractValidationFailedException : Exception
{
    public int ErrorCode { get; set; }

    public ContractValidationFailedException(int errorCode, string message) : base(message) { ErrorCode = errorCode; }

    public ContractValidationFailedException(string message) : base(message) { }

    public ContractValidationFailedException(string message, Exception innerException) : base(message, innerException) { }

    public ContractValidationFailedException() { }
}