using Backend.Dtos.Requests.Category;
using Backend.Dtos.Responses;
using Backend.Dtos.Responses.Category;

namespace Backend.Services.Interfaces
{
    public interface CategoryServInterface
    {
        Task<ApiResponse<IEnumerable<CategoryResponse>>> GetAllAsync();
        Task<ApiResponse<CategoryResponse>> GetByIdAsync(int id);
        Task<ApiResponse<CategoryResponse>> CreateAsync(CategoryCreateRequest categoryCreateRequest);
        Task<ApiResponse<CategoryResponse>> UpdateAsync(int id, CategoryUpdateRequest categoryUpdateRequest);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
