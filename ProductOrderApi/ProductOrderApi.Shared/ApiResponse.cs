public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string msg = "Success") =>
        new() { Success = true, Data = data, Message = msg };

    public static ApiResponse<T> Fail(string msg) =>
        new() { Success = false, Data = default, Message = msg };
}
