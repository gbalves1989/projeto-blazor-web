using Backend.Database;
using Backend.Database.Models;
using Backend.Database.Repositories;
using Backend.Tests.Helpers;
using Xunit;

namespace Backend.Tests.Repositories
{
    public class TestUserRepository : IDisposable
    {
        private readonly UserRepository _repository;
        private readonly ApplicationDbContext _context;

        public TestUserRepository()
        {
            _context = TestApplicationDbContext.CreateContextWithData();
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnOnlyActiveUsers()
        {
            var result = await _repository.GetAllAsync();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); 
            Assert.All(result, u => Assert.True(u.Active));
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
        {
            var userId = 1;
            var result = await _repository.GetByIdAsync(userId);
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("João Silva", result.Name);
            Assert.True(result.Active);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            var userId = 999;
            var result = await _repository.GetByIdAsync(userId);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_WithInactiveUser_ShouldReturnNull()
        {
            var userId = 3; 
            var result = await _repository.GetByIdAsync(userId);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
        {
            var email = "joao@teste.com";
            var result = await _repository.GetByEmailAsync(email);
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.True(result.Active);
        }

        [Fact]
        public async Task GetByEmailAsync_WithInvalidEmail_ShouldReturnNull()
        {
            var email = "inexistente@teste.com";
            var result = await _repository.GetByEmailAsync(email);
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_WithValidUser_ShouldCreateUser()
        {
            var newUser = new User
            {
                Name = "Novo Usuário",
                Email = "novo@teste.com",
                Password = "senhaHash",
                Created_At = DateTime.UtcNow,
                Active = true
            };

            var result = await _repository.CreateAsync(newUser);
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Novo Usuário", result.Name);
            Assert.Equal("novo@teste.com", result.Email);

            var savedUser = await _repository.GetByEmailAsync("novo@teste.com");
            Assert.NotNull(savedUser);
        }

        [Fact]
        public async Task UpdateAsync_WithValidUser_ShouldUpdateUser()
        {
            var user = await _repository.GetByIdAsync(1);
            user.Name = "João Silva Atualizado";
            user.Email = "joao.atualizado@teste.com";

            var result = await _repository.UpdateAsync(user);
            Assert.NotNull(result);
            Assert.Equal("João Silva Atualizado", result.Name);
            Assert.Equal("joao.atualizado@teste.com", result.Email);

            var updatedUser = await _repository.GetByIdAsync(1);
            Assert.Equal("João Silva Atualizado", updatedUser.Name);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldSoftDeleteUser()
        {
            var userId = 1;
            var result = await _repository.DeleteAsync(userId);
            Assert.True(result);

            var deletedUser = _context.Users.Find(userId);
            Assert.NotNull(deletedUser);
            Assert.False(deletedUser.Active);

            var userFromRepository = await _repository.GetByIdAsync(userId);
            Assert.Null(userFromRepository);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
        {
            var userId = 999;
            var result = await _repository.DeleteAsync(userId);
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
        {
            var userId = 1;
            var result = await _repository.ExistsAsync(userId);
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
        {
            var userId = 999;
            var result = await _repository.ExistsAsync(userId);
            Assert.False(result);
        }

        [Fact]
        public async Task EmailExistsAsync_WithExistingEmail_ShouldReturnTrue()
        {
            var email = "joao@teste.com";
            var result = await _repository.EmailExistsAsync(email);
            Assert.True(result);
        }

        [Fact]
        public async Task EmailExistsAsync_WithNonExistingEmail_ShouldReturnFalse()
        {
            var email = "inexistente@teste.com";
            var result = await _repository.EmailExistsAsync(email);
            Assert.False(result);
        }

        [Fact]
        public async Task EmailExistsAsync_WithExcludeId_ShouldExcludeSpecifiedUser()
        {
            var email = "joao@teste.com";
            var excludeId = 1; 
            var result = await _repository.EmailExistsAsync(email, excludeId);

            Assert.False(result); 
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
