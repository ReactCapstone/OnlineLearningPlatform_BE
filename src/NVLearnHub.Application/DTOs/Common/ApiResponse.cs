namespace NVLearnHub.Application.DTOs.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = default!;
    public T? Data { get; set; }

    // Success with data
    public ApiResponse(T data, string message = "Request successful.")
    {
        Success = true;
        Message = message;
        Data = data;
    }

    // Success without data (e.g. delete, reset password)
    public ApiResponse(string message)
    {
        Success = true;
        Message = message;
        Data = default;
    }

    // Failure
    public ApiResponse(bool success, string message)
    {
        Success = success;
        Message = message;
        Data = default;
    }
}