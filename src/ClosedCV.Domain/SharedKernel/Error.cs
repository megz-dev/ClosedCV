namespace ClosedCV.Domain.SharedKernel;

public record Error(string Code, string Message)
{
    private const string NullValueCode = "Error.NullValue";
    private const string UnAuthorizedCode = "Error.UnAuthorized";
    private const string NotFoundCodeSuffix = ".NotFound";
    private const string ValidationCodePrefix = "Validation.";
    private const string InternalErrorCode = "Error.Internal";
    private const string ConflictCode = "Error.Conflict";

    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new(NullValueCode, "Value shouldn't be null");
    public static readonly Error UnAuthorized = new(UnAuthorizedCode, "User is unauthorized to do this action");
    public static Error NotFound(string entity, object id) => new($"{entity}{NotFoundCodeSuffix}", $"Requested {entity} with Id '{id}' was not found");
    public static Error Validation(string field, string message) => new($"{ValidationCodePrefix}{field}", message);
    public static Error Conflict(string message) => new(ConflictCode, message);
    public static Error InternalError(string message) => new(InternalErrorCode, message);


    // Helper properties to identify error types based on the code
    public bool IsUnAuthorized => Code.Equals(UnAuthorizedCode, StringComparison.OrdinalIgnoreCase);
    public bool IsNotFound => Code.EndsWith(NotFoundCodeSuffix, StringComparison.OrdinalIgnoreCase);
    public bool IsConflict => Code.Equals(ConflictCode, StringComparison.OrdinalIgnoreCase);
    public bool IsValidationError => Code.StartsWith(ValidationCodePrefix, StringComparison.OrdinalIgnoreCase);
    public bool IsInternalError => Code.Equals(InternalErrorCode, StringComparison.OrdinalIgnoreCase);
}
