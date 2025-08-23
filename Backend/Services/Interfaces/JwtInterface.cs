using Backend.Database.Models;

namespace Backend.Services.Interfaces
{
    public interface JwtInterface
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
    }
}
