namespace ApiContracts.Extensions.Exceptions;

/// <summary>
/// The contract validation failed exception class that handles the contract validation failed exception.
/// </summary>
public class ContractValidationFailedException : Exception
{
    /// <summary>
    /// The error code of the exception.
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// The contract validation failed exception constructor.
    /// </summary>
    /// <param name="errorCode">The error code of the exception</param>
    /// <param name="message">The exception message</param>
    public ContractValidationFailedException(int errorCode, string message) : base(message) { ErrorCode = errorCode; }

    /// <summary>
    /// The contract validation failed exception constructor.
    /// </summary>
    /// <param name="message">The exception message</param>
    public ContractValidationFailedException(string message) : base(message) { }

    /// <summary>
    /// The contract validation failed exception constructor.
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="innerException">The inner exception of the exception</param>
    public ContractValidationFailedException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// The contract validation failed exception constructor.
    /// </summary>
    public ContractValidationFailedException() { }
}