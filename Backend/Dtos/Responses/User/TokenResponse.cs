namespace Backend.Dtos.Responses.User
{
    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserResponse UserResponse { get; set; } = new();
    }
}
