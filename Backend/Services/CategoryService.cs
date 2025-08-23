using Backend.Database.Interfaces;
using Backend.Dtos.Requests.Category;
using Backend.Dtos.Responses;
using Backend.Dtos.Responses.Category;
using Backend.Services.Interfaces;
using Backend.Database.Models;

namespace Backend.Services
{
    public class CategoryService : CategoryServInterface
    {
        private readonly CategoryInterface _repository;

        public CategoryService(CategoryInterface repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<CategoryResponse>> CreateAsync(CategoryCreateRequest categoryCreateRequest)
        {
            try
            {
                if (await _repository.NameExistsAsync(categoryCreateRequest.Name))
                    return ApiResponse<CategoryResponse>.BadRequest("Nome da categoria já está em uso");

                var category = new Category
                {
                    Name = categoryCreateRequest.Name,
                    Description = categoryCreateRequest.Description,
                    Created_At = DateTime.UtcNow,
                    Active = true
                };

                var categoryCreated = await _repository.CreateAsync(category);

                var categoryResponse = new CategoryResponse
                {
                    Id = categoryCreated.Id,
                    Name = categoryCreated.Name,
                    Description = categoryCreated.Description,
                    Created_At = categoryCreated.Created_At,
                    Active = categoryCreated.Active
                };

                return ApiResponse<CategoryResponse>.Created(categoryResponse, "Categoria criada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryResponse>.InternalServerError($"Erro ao criar categoria: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                if (!await _repository.ExistsAsync(id))
                    return ApiResponse<bool>.NotFound("Categoria não encontrada");

                var result = await _repository.DeleteAsync(id);
                return ApiResponse<bool>.Ok(result, "Categoria excluída com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.InternalServerError($"Erro ao excluir categoria: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<CategoryResponse>>> GetAllAsync()
        {
            try
            {
                var categories = await _repository.GetAllAsync();
                var categoriesDto = categories.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Created_At = c.Created_At,
                    Active = c.Active
                });

                return ApiResponse<IEnumerable<CategoryResponse>>.Ok(categoriesDto, "Categorias recuperadas com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CategoryResponse>>.InternalServerError($"Erro ao recuperar categorias: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CategoryResponse>> GetByIdAsync(int id)
        {
            try
            {
                var category = await _repository.GetByIdAsync(id);
                if (category == null)
                    return ApiResponse<CategoryResponse>.NotFound("Categoria não encontrada");

                var categoryResponse = new CategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Created_At = category.Created_At,
                    Active = category.Active
                };

                return ApiResponse<CategoryResponse>.Ok(categoryResponse, "Categoria recuperada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryResponse>.InternalServerError($"Erro ao recuperar categoria: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CategoryResponse>> UpdateAsync(int id, CategoryUpdateRequest categoryUpdateRequest)
        {
            try
            {
                var category = await _repository.GetByIdAsync(id);
                if (category == null)
                    return ApiResponse<CategoryResponse>.NotFound("Categoria não encontrada");

                if (await _repository.NameExistsAsync(categoryUpdateRequest.Name, id))
                    return ApiResponse<CategoryResponse>.BadRequest("Nome da categoria já está em uso");

                category.Name = categoryUpdateRequest.Name;
                category.Description = categoryUpdateRequest.Description;
                category.Active = categoryUpdateRequest.Active;

                var categoryUpdated = await _repository.UpdateAsync(category);

                var categoryResponse = new CategoryResponse
                {
                    Id = categoryUpdated.Id,
                    Name = categoryUpdated.Name,
                    Description = categoryUpdated.Description,
                    Created_At = categoryUpdated.Created_At,
                    Active = categoryUpdated.Active
                };

                return ApiResponse<CategoryResponse>.Ok(categoryResponse, "Categoria atualizada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoryResponse>.InternalServerError($"Erro ao atualizar categoria: {ex.Message}");
            }
        }
    }
}
