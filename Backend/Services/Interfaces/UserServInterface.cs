using Backend.Dtos.Requests.User;
using Backend.Dtos.Responses;
using Backend.Dtos.Responses.User;

namespace Backend.Services.Interfaces
{
    public interface UserServInterface
    {
        Task<ApiResponse<IEnumerable<UserResponse>>> GetAllAsync();
        Task<ApiResponse<UserResponse>> GetByIdAsync(int id);
        Task<ApiResponse<UserResponse>> CreateAsync(UserCreateRequest userCreateRequest);
        Task<ApiResponse<UserResponse>> UpdateAsync(int id, UserUpdateRequest userUpdateRequest);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<TokenResponse>> LoginAsync(UserLoginRequest userLoginRequest);
    }
}
