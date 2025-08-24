using Backend.Database.Interfaces;
using Backend.Database.Models;
using Backend.Dtos.Requests.User;
using Backend.Services;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Moq;
using Xunit;

namespace Backend.Tests.Services
{
    public class TestUserService
    {
        private readonly Mock<UserInterface> _mockUsuarioRepository;
        private readonly Mock<JwtInterface> _mockJwtService;
        private readonly UserService _usuarioService;

        public TestUserService()
        {
            _mockUsuarioRepository = new Mock<UserInterface>();
            _mockJwtService = new Mock<JwtInterface>();
            _usuarioService = new UserService(_mockUsuarioRepository.Object, _mockJwtService.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnSuccessResponse()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Name = "João", Email = "joao@teste.com", Created_At = DateTime.UtcNow, Active = true },
                new User { Id = 2, Name = "Maria", Email = "maria@teste.com", Created_At = DateTime.UtcNow, Active = true }
            };

            _mockUsuarioRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _usuarioService.GetAllAsync();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
        {
            // Arrange
            var user = new User { Id = 1, Name = "João", Email = "joao@teste.com", Created_At = DateTime.UtcNow, Active = true };
            _mockUsuarioRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _usuarioService.GetByIdAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal("João", result.Data.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            _mockUsuarioRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User)null);

            // Act
            var result = await _usuarioService.GetByIdAsync(999);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Usuário não encontrado", result.Message);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var createDto = new UserCreateRequest
            {
                Name = "Novo Usuário",
                Email = "novo@teste.com",
                Password = "123456"
            };

            var createdUser = new User
            {
                Id = 1,
                Name = createDto.Name,
                Email = createDto.Email,
                Password = "hashedPassword",
                Created_At = DateTime.UtcNow,
                Active = true
            };

            _mockUsuarioRepository.Setup(r => r.EmailExistsAsync(createDto.Email, null)).ReturnsAsync(false);
            _mockUsuarioRepository.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

            // Act
            var result = await _usuarioService.CreateAsync(createDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(201, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal("Novo Usuário", result.Data.Name);
            Assert.Equal("novo@teste.com", result.Data.Email);
        }

        [Fact]
        public async Task CreateAsync_WithExistingEmail_ShouldReturnBadRequest()
        {
            // Arrange
            var createDto = new UserCreateRequest
            {
                Name = "Novo Usuário",
                Email = "existente@teste.com",
                Password = "123456"
            };

            _mockUsuarioRepository.Setup(r => r.EmailExistsAsync(createDto.Email, null)).ReturnsAsync(true);

            // Act
            var result = await _usuarioService.CreateAsync(createDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Email já está em uso", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateUser()
        {
            // Arrange
            var updateDto = new UserUpdateRequest
            {
                Name = "João Atualizado",
                Email = "joao.novo@teste.com",
                Active = true
            };

            var existingUser = new User
            {
                Id = 1,
                Name = "João",
                Email = "joao@teste.com",
                Password = "hashedPassword",
                Created_At = DateTime.UtcNow,
                Active = true
            };

            var updatedUser = new User
            {
                Id = 1,
                Name = updateDto.Name,
                Email = updateDto.Email,
                Password = "hashedPassword",
                Created_At = DateTime.UtcNow,
                Active = updateDto.Active
            };

            _mockUsuarioRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingUser);
            _mockUsuarioRepository.Setup(r => r.EmailExistsAsync(updateDto.Email, 1)).ReturnsAsync(false);
            _mockUsuarioRepository.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(updatedUser);

            // Act
            var result = await _usuarioService.UpdateAsync(1, updateDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal("João Atualizado", result.Data.Name);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentUser_ShouldReturnNotFound()
        {
            // Arrange
            var updateDto = new UserUpdateRequest
            {
                Name = "João Atualizado",
                Email = "joao.novo@teste.com",
                Active = true
            };

            _mockUsuarioRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User)null);

            // Act
            var result = await _usuarioService.UpdateAsync(999, updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Usuário não encontrado", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteUser()
        {
            // Arrange
            _mockUsuarioRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            _mockUsuarioRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _usuarioService.DeleteAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            _mockUsuarioRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _usuarioService.DeleteAsync(999);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Usuário não encontrado", result.Message);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var loginDto = new LoginRequest
            {
                Email = "joao@teste.com",
                Password = "123456"
            };

            var user = new User
            {
                Id = 1,
                Name = "João",
                Email = "joao@teste.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Created_At = DateTime.UtcNow,
                Active = true
            };

            var token = "jwt-token-example";

            _mockUsuarioRepository.Setup(r => r.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);
            _mockJwtService.Setup(j => j.GenerateToken(user)).Returns(token);

            // Act
            var result = await _usuarioService.LoginAsync(loginDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(token, result.Data.Token);
            Assert.Equal("João", result.Data.UserResponse.Name);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ShouldReturnBadRequest()
        {
            // Arrange
            var loginDto = new LoginRequest
            {
                Email = "joao@teste.com",
                Password = "senhaErrada"
            };

            var user = new User
            {
                Id = 1,
                Name = "João",
                Email = "joao@teste.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Created_At = DateTime.UtcNow,
                Active = true
            };

            _mockUsuarioRepository.Setup(r => r.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);

            // Act
            var result = await _usuarioService.LoginAsync(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Email ou senha inválidos", result.Message);
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentUser_ShouldReturnBadRequest()
        {
            // Arrange
            var loginDto = new LoginRequest
            {
                Email = "inexistente@teste.com",
                Password = "123456"
            };

            _mockUsuarioRepository.Setup(r => r.GetByEmailAsync(loginDto.Email)).ReturnsAsync((User)null);

            // Act
            var result = await _usuarioService.LoginAsync(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Email ou senha inválidos", result.Message);
        }
    }
}
