namespace WebShoppingUI.DataServices;

public record ServiceCallResult
{
    public bool IsSuccess => ErrorDetail is null;
    public ServiceCallError? ErrorType { get; init; }
    public string? ErrorDetail { get; init; }

    public bool IsUnauthorized => ErrorType == ServiceCallError.Unauthorized;
    public bool IsNotFound => ErrorType == ServiceCallError.NotFound;
    public bool IsConflict => ErrorType == ServiceCallError.Conflict;
    public bool IsBadRequest => ErrorType == ServiceCallError.BadRequest;
    public bool IsInternalError => ErrorType == ServiceCallError.InternalError;

    public ServiceCallResult(ServiceCallError? errorType = null, string? errorMessage = null)
    {
        ErrorType = errorType;
        ErrorDetail = errorMessage;
    }

    public static ServiceCallResult Success => new();

    public static ServiceCallResult Unauthorized
        => new(ServiceCallError.Unauthorized, UnauthorizedMessage);

    public static ServiceCallResult NotFound(string detail)
        => new(ServiceCallError.NotFound, detail);

    public static ServiceCallResult BadRequest(string detail)
        => new(ServiceCallError.BadRequest, detail);

    public static ServiceCallResult Conflict(string detail)
        => new(ServiceCallError.Conflict, detail);

    public static ServiceCallResult InternalError
        => new(ServiceCallError.InternalError, InternalErrorMessage);

    public static ServiceCallResult UntypedError(string errorMsg) => new(null, errorMsg);

    protected const string UnauthorizedMessage = "Request unauthorized";
    protected const string InternalErrorMessage = "Internal service error";

    // conversions
    public ServiceCallResult<T> WithValue<T>(T? value = null) where T : class
        => new(ErrorType, ErrorDetail, value);
}

public record ServiceCallResult<T> : ServiceCallResult
    where T : class
{
    public T? Value
    {
        get => IsSuccess ? _value : null;
    }
    private readonly T? _value;

    public ServiceCallResult(ServiceCallError? errorType, string? errorMessage = null, T? result = null)
        : base(errorType, errorMessage)
    {
        if (!IsSuccess && result is not null)
        {
            throw new ArgumentException("Unsuccessful request cannot have a result.", nameof(result));
        }
        _value = result;
    }

    // shorthands
    public static new ServiceCallResult<T> Success(T value)
        => new(null, null, value);

    public static new ServiceCallResult<T> Unauthorized
        => new(ServiceCallError.Unauthorized);

    public static new ServiceCallResult<T> NotFound(string? detail)
        => new(ServiceCallError.NotFound, detail);

    public static new ServiceCallResult<T> BadRequest(string detail)
        => new(ServiceCallError.BadRequest, detail);

    public static new ServiceCallResult<T> Conflict(string detail)
        => new(ServiceCallError.Conflict, detail);

    public static new ServiceCallResult<T> InternalError
        => new(ServiceCallError.InternalError, InternalErrorMessage);

    public static new ServiceCallResult<T> UntypedError(string errorMsg) => new(null, errorMsg);

    // conversions
    public static implicit operator ServiceCallResult<T>(T result) => Success(result);
}

public enum ServiceCallError
{
    Unauthorized,
    NotFound,
    Conflict,
    BadRequest,
    InternalError,
}