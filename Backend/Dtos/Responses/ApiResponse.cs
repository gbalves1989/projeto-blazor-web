namespace Backend.Dtos.Responses
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public bool Success => StatusCode >= 200 && StatusCode < 300;

        public ApiResponse()
        {
        }

        public ApiResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public ApiResponse(int statusCode, string message, T data)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        public static ApiResponse<T> Ok(T data, string message = "Sucesso")
        {
            return new ApiResponse<T>(200, message, data);
        }

        public static ApiResponse<T> Created(T data, string message = "Criado com sucesso")
        {
            return new ApiResponse<T>(201, message, data);
        }

        public static ApiResponse<T> BadRequest(string message = "Requisição inválida")
        {
            return new ApiResponse<T>(400, message);
        }

        public static ApiResponse<T> NotFound(string message = "Não encontrado")
        {
            return new ApiResponse<T>(404, message);
        }

        public static ApiResponse<T> InternalServerError(string message = "Erro interno do servidor")
        {
            return new ApiResponse<T>(500, message);
        }
    }
}
