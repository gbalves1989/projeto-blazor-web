using Backend.Dtos.Requests.Category;
using Backend.Dtos.Responses;
using Backend.Dtos.Responses.Category;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly CategoryServInterface _service;

        public CategoryController(CategoryServInterface service)
        {
            _service = service;
        }

        /// <summary>
        /// Obter todas as categorias
        /// </summary>
        /// <returns>Lista de categorias</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CategoryResponse>>>> GetAll()
        {
            var response = await _service.GetAllAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obter categoria por ID
        /// </summary>
        /// <param name="id">ID da categoria</param>
        /// <returns>Categoria encontrada</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Criar nova categoria
        /// </summary>
        /// <param name="categoryCreateRequest">Dados da categoria</param>
        /// <returns>Categoria criada</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> Create([FromBody] CategoryCreateRequest categoryCreateRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var response = ApiResponse<CategoryResponse>.BadRequest($"Dados inválidos: {string.Join(", ", errors)}");
                return StatusCode(response.StatusCode, response);
            }

            var result = await _service.CreateAsync(categoryCreateRequest);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Atualizar categoria
        /// </summary>
        /// <param name="id">ID da categoria</param>
        /// <param name="categoryUpdateRequest">Dados atualizados</param>
        /// <returns>Categoria atualizada</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> Update(int id, [FromBody] CategoryUpdateRequest categoryUpdateRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var response = ApiResponse<CategoryResponse>.BadRequest($"Dados inválidos: {string.Join(", ", errors)}");
                return StatusCode(response.StatusCode, response);
            }

            var result = await _service.UpdateAsync(id, categoryUpdateRequest);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Excluir categoria
        /// </summary>
        /// <param name="id">ID da categoria</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var response = await _service.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
