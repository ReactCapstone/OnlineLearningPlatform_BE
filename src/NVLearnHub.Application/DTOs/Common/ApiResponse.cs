public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = default!;
    public T? Data { get; set; }
    public int StatusCode { get; set; }

    public ApiResponse(T data, string message = "Request successful.")
    {
        Success = true;
        Message = message;
        Data = data;
        StatusCode = 200;
    }

    public ApiResponse(string message)
    {
        Success = true;
        Message = message;
        Data = default;
        StatusCode = 200;
    }

    public ApiResponse(bool success, string message, int statusCode = 400)
    {
        Success = success;
        Message = message;
        Data = default;
        StatusCode = statusCode;
    }
}