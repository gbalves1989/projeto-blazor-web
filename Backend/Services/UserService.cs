using Backend.Database.Interfaces;
using Backend.Database.Models;
using Backend.Dtos.Requests.User;
using Backend.Dtos.Responses;
using Backend.Dtos.Responses.User;
using Backend.Services.Interfaces;

namespace Backend.Services
{
    public class UserService : UserServInterface
    {
        private readonly UserInterface _repository;
        private readonly JwtInterface _jwtService;

        public UserService(UserInterface repository, JwtInterface jwtService)
        {
            _repository = repository;
            _jwtService = jwtService;
        }

        public async Task<ApiResponse<UserResponse>> CreateAsync(UserCreateRequest userCreateRequest)
        {
            try
            {
                if (await _repository.EmailExistsAsync(userCreateRequest.Email))
                    return ApiResponse<UserResponse>.BadRequest("Email já está em uso");

                var user = new User
                {
                    Name = userCreateRequest.Name,
                    Email = userCreateRequest.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(userCreateRequest.Password),
                    Created_At = DateTime.UtcNow,
                    Active = true
                };

                var userCreated = await _repository.CreateAsync(user);

                var userResponse = new UserResponse
                {
                    Id = userCreated.Id,
                    Name = userCreated.Name,
                    Email = userCreated.Email,
                    Created_At = userCreated.Created_At,
                    Active = userCreated.Active
                };

                return ApiResponse<UserResponse>.Created(userResponse, "Usuário criado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserResponse>.InternalServerError($"Erro ao criar usuário: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                if (!await _repository.ExistsAsync(id))
                    return ApiResponse<bool>.NotFound("Usuário não encontrado");

                var resultado = await _repository.DeleteAsync(id);
                return ApiResponse<bool>.Ok(resultado, "Usuário excluído com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.InternalServerError($"Erro ao excluir usuário: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<UserResponse>>> GetAllAsync()
        {
            try
            {
                var users = await _repository.GetAllAsync();
                var usersResponse = users.Select(u => new UserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Created_At = u.Created_At,
                    Active = u.Active
                });

                return ApiResponse<IEnumerable<UserResponse>>.Ok(usersResponse, "Usuários recuperados com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<UserResponse>>.InternalServerError($"Erro ao recuperar usuários: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserResponse>> GetByIdAsync(int id)
        {
            try
            {
                var user = await _repository.GetByIdAsync(id);
                if (user == null)
                    return ApiResponse<UserResponse>.NotFound("Usuário não encontrado");

                var userResponse = new UserResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Created_At = user.Created_At,
                    Active = user.Active
                };

                return ApiResponse<UserResponse>.Ok(userResponse, "Usuário recuperado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserResponse>.InternalServerError($"Erro ao recuperar usuário: {ex.Message}");
            }
        }

        public async Task<ApiResponse<TokenResponse>> LoginAsync(UserLoginRequest userLoginRequest)
        {
            try
            {
                var user = await _repository.GetByEmailAsync(userLoginRequest.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(userLoginRequest.Password, user.Password))
                    return ApiResponse<TokenResponse>.BadRequest("Email ou senha inválidos");

                var token = _jwtService.GenerateToken(user);

                var loginResponse = new TokenResponse
                {
                    Token = token,
                    UserResponse = new UserResponse
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Created_At = user.Created_At,
                        Active = user.Active
                    }
                };

                return ApiResponse<TokenResponse>.Ok(loginResponse, "Login realizado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<TokenResponse>.InternalServerError($"Erro ao realizar login: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserResponse>> UpdateAsync(int id, UserUpdateRequest userUpdateRequest)
        {
            try
            {
                var user = await _repository.GetByIdAsync(id);
                if (user == null)
                    return ApiResponse<UserResponse>.NotFound("Usuário não encontrado");

                // Verificar se email já existe (excluindo o usuário atual)
                if (await _repository.EmailExistsAsync(userUpdateRequest.Email, id))
                    return ApiResponse<UserResponse>.BadRequest("Email já está em uso");

                user.Name = userUpdateRequest.Name;
                user.Email = userUpdateRequest.Email;
                user.Active = userUpdateRequest.Active;

                var userUpdated = await _repository.UpdateAsync(user);

                var userResponse = new UserResponse
                {
                    Id = userUpdated.Id,
                    Name = userUpdated.Name,
                    Email = userUpdated.Email,
                    Created_At = userUpdated.Created_At,
                    Active = userUpdated.Active
                };

                return ApiResponse<UserResponse>.Ok(userResponse, "Usuário atualizado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserResponse>.InternalServerError($"Erro ao atualizar usuário: {ex.Message}");
            }
        }
    }
}
