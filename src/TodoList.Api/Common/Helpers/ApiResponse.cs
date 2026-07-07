using System.Text.Json.Serialization;

namespace TodoList.Api.Shared.Presentation.Helpers;

public class ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("error")]
    public ApiError? Error { get; set; }

    public static ApiResponse<T> SuccessResult(
        T data,
        string message = "Process completed successfully."
    )
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Error = null,
        };
    }

    public static ApiResponse<T> FailureResult(int statusCode, string message, string details)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Error = new ApiError { StatusCode = statusCode, Details = details },
        };
    }
}

public class ApiError
{
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("details")]
    public string Details { get; set; } = string.Empty;
}
