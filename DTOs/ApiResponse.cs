namespace net_api.DTOs
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public ApiResponse(int statusCode, string? message = null, T? data = default)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        // Success responses
        public static ApiResponse<T> Success(T data, string? message = "Success")
        {
            return new ApiResponse<T>(200, message, data);
        }

        public static ApiResponse<T> Created(T data, string? message = "Resource created successfully")
        {
            return new ApiResponse<T>(201, message, data);
        }

        // Error responses
        public static ApiResponse<T> NotFound(string message = "Resource not found")
        {
            return new ApiResponse<T>(404, message, default);
        }

        public static ApiResponse<T> BadRequest(string message = "Bad request")
        {
            return new ApiResponse<T>(400, message, default);
        }

        public static ApiResponse<T> Unauthorized(string message = "Unauthorized")
        {
            return new ApiResponse<T>(401, message, default);
        }
    }

    // Non-generic version for responses without data
    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse(int statusCode, string? message = null, object? data = null)
            : base(statusCode, message, data)
        {
        }

        public static ApiResponse Success(string? message = "Success")
        {
            return new ApiResponse(200, message);
        }

        public static ApiResponse Created(string? message = "Resource created successfully")
        {
            return new ApiResponse(201, message);
        }

        public new static ApiResponse NotFound(string message = "Resource not found")
        {
            return new ApiResponse(404, message);
        }

        public new static ApiResponse BadRequest(string message = "Bad request")
        {
            return new ApiResponse(400, message);
        }

        public new static ApiResponse Unauthorized(string message = "Unauthorized")
        {
            return new ApiResponse(401, message);
        }
    }
}
