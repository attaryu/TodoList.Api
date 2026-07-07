namespace TodoList.Api.Common.Helpers;

public static class ApiResponseHelper
{
    public static ApiResponse<T> Success<T>(
        T data,
        string message = "Process completed successfully."
    )
    {
        return ApiResponse<T>.SuccessResult(data, message);
    }

    public static ApiResponse<object> Error(int statusCode, string message, string details)
    {
        return ApiResponse<object>.FailureResult(statusCode, message, details);
    }
}
