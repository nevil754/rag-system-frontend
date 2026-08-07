namespace RagSystemFrontend.Core.DTOs.Common;

/// <summary>
/// Esito di una chiamata al backend RAG, senza payload (es. DELETE/PATCH -> 204).
/// Evita di propagare eccezioni per errori HTTP attesi (401/403/404/400/409/422/429/5xx).
/// </summary>
public class ApiResult
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string? ErrorMessage { get; init; }
    public int? RetryAfterSeconds { get; init; }

    public bool IsUnauthorized => StatusCode == 401;
    public bool IsForbidden => StatusCode == 403;
    public bool IsNotFound => StatusCode == 404;

    public static ApiResult Ok(int statusCode = 200) => new() { Success = true, StatusCode = statusCode };

    public static ApiResult Fail(int statusCode, string errorMessage, int? retryAfterSeconds = null) =>
        new() { Success = false, StatusCode = statusCode, ErrorMessage = errorMessage, RetryAfterSeconds = retryAfterSeconds };
}

/// <summary>
/// Esito di una chiamata al backend RAG con payload deserializzato.
/// </summary>
public sealed class ApiResult<T> : ApiResult
{
    public T? Data { get; init; }

    public static ApiResult<T> Ok(T data, int statusCode = 200) =>
        new() { Success = true, StatusCode = statusCode, Data = data };

    public static new ApiResult<T> Fail(int statusCode, string errorMessage, int? retryAfterSeconds = null) =>
        new() { Success = false, StatusCode = statusCode, ErrorMessage = errorMessage, RetryAfterSeconds = retryAfterSeconds };
}
