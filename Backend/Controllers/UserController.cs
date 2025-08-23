using Backend.Dtos.Requests.User;
using Backend.Dtos.Responses;
using Backend.Dtos.Responses.User;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserServInterface _userService;

        public UserController(UserServInterface userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Obter todos os usuários
        /// </summary>
        /// <returns>Lista de usuários</returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserResponse>>>> GetAll()
        {
            var response = await _userService.GetAllAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Obter usuário por ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Usuário encontrado</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserResponse>>> GetById(int id)
        {
            var response = await _userService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Criar novo usuário
        /// </summary>
        /// <param name="userCreateRequest">Dados do usuário</param>
        /// <returns>Usuário criado</returns>
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> Create([FromBody] UserCreateRequest userCreateRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var response = ApiResponse<UserResponse>.BadRequest($"Dados inválidos: {string.Join(", ", errors)}");
                return StatusCode(response.StatusCode, response);
            }

            var result = await _userService.CreateAsync(userCreateRequest);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Atualizar usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="userUpdateRequest">Dados atualizados</param>
        /// <returns>Usuário atualizado</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserResponse>>> Update(int id, [FromBody] UserUpdateRequest userUpdateRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var response = ApiResponse<UserResponse>.BadRequest($"Dados inválidos: {string.Join(", ", errors)}");
                return StatusCode(response.StatusCode, response);
            }

            var result = await _userService.UpdateAsync(id, userUpdateRequest);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Excluir usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var response = await _userService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Realizar login
        /// </summary>
        /// <param name="loginRequest">Dados de login</param>
        /// <returns>Token de autenticação</returns>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<TokenResponse>>> Login([FromBody] UserLoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var response = ApiResponse<TokenResponse>.BadRequest($"Dados inválidos: {string.Join(", ", errors)}");
                return StatusCode(response.StatusCode, response);
            }

            var result = await _userService.LoginAsync(loginRequest);
            return StatusCode(result.StatusCode, result);
        }
    }
}
