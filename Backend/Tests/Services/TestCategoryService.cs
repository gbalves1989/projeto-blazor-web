using Backend.Database.Interfaces;
using Backend.Database.Models;
using Backend.Dtos.Requests.Category;
using Backend.Services;
using Moq;
using Xunit;

namespace Backend.Tests.Services
{
    public class TestCategoryService
    {
        private readonly Mock<CategoryInterface> _mockCategoriaRepository;
        private readonly CategoryService _categoriaService;

        public TestCategoryService()
        {
            _mockCategoriaRepository = new Mock<CategoryInterface>();
            _categoriaService = new CategoryService(_mockCategoriaRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnSuccessResponse()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Tecnologia", Description = "Desc 1", Created_At = DateTime.UtcNow, Active = true },
                new Category { Id = 2, Name = "Esportes", Description = "Desc 2", Created_At = DateTime.UtcNow, Active = true }
            };

            _mockCategoriaRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            // Act
            var result = await _categoriaService.GetAllAsync();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Tecnologia", Description = "Desc", Created_At = DateTime.UtcNow, Active = true };
            _mockCategoriaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

            // Act
            var result = await _categoriaService.GetByIdAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal("Tecnologia", result.Data.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            _mockCategoriaRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Category)null);

            // Act
            var result = await _categoriaService.GetByIdAsync(999);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Categoria não encontrada", result.Message);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateCategory()
        {
            // Arrange
            var createDto = new CategoryCreateRequest
            {
                Name = "Nova Categoria",
                Description = "Descrição da nova categoria"
            };

            var createdCategory = new Category
            {
                Id = 1,
                Name = createDto.Name,
                Description = createDto.Description,
                Created_At = DateTime.UtcNow,
                Active = true
            };

            _mockCategoriaRepository.Setup(r => r.NameExistsAsync(createDto.Name, null)).ReturnsAsync(false);
            _mockCategoriaRepository.Setup(r => r.CreateAsync(It.IsAny<Category>())).ReturnsAsync(createdCategory);

            // Act
            var result = await _categoriaService.CreateAsync(createDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(201, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal("Nova Categoria", result.Data.Name);
            Assert.Equal("Descrição da nova categoria", result.Data.Description);
        }

        [Fact]
        public async Task CreateAsync_WithExistingName_ShouldReturnBadRequest()
        {
            // Arrange
            var createDto = new CategoryCreateRequest
            {
                Name = "Categoria Existente",
                Description = "Descrição"
            };

            _mockCategoriaRepository.Setup(r => r.NameExistsAsync(createDto.Name, null)).ReturnsAsync(true);

            // Act
            var result = await _categoriaService.CreateAsync(createDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Nome da categoria já está em uso", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateCategory()
        {
            // Arrange
            var updateDto = new CategoryUpdateRequest
            {
                Name = "Categoria Atualizada",
                Description = "Descrição atualizada",
                Active = true
            };

            var existingCategory = new Category
            {
                Id = 1,
                Name = "Categoria Original",
                Description = "Descrição original",
                Created_At = DateTime.UtcNow,
                Active = true
            };

            var updatedCategory = new Category
            {
                Id = 1,
                Name = updateDto.Name,
                Description = updateDto.Description,
                Created_At = DateTime.UtcNow,
                Active = updateDto.Active
            };

            _mockCategoriaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingCategory);
            _mockCategoriaRepository.Setup(r => r.NameExistsAsync(updateDto.Name, 1)).ReturnsAsync(false);
            _mockCategoriaRepository.Setup(r => r.UpdateAsync(It.IsAny<Category>())).ReturnsAsync(updatedCategory);

            // Act
            var result = await _categoriaService.UpdateAsync(1, updateDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal("Categoria Atualizada", result.Data.Name);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentCategory_ShouldReturnNotFound()
        {
            // Arrange
            var updateDto = new CategoryUpdateRequest
            {
                Name = "Categoria Atualizada",
                Description = "Descrição atualizada",
                Active = true
            };

            _mockCategoriaRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Category)null);

            // Act
            var result = await _categoriaService.UpdateAsync(999, updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Categoria não encontrada", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_WithExistingName_ShouldReturnBadRequest()
        {
            // Arrange
            var updateDto = new CategoryUpdateRequest
            {
                Name = "Nome Existente",
                Description = "Descrição atualizada",
                Active = true
            };

            var existingCategory = new Category
            {
                Id = 1,
                Name = "Categoria Original",
                Description = "Descrição original",
                Created_At = DateTime.UtcNow,
                Active = true
            };

            _mockCategoriaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingCategory);
            _mockCategoriaRepository.Setup(r => r.NameExistsAsync(updateDto.Name, 1)).ReturnsAsync(true);

            // Act
            var result = await _categoriaService.UpdateAsync(1, updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Nome da categoria já está em uso", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteCategory()
        {
            // Arrange
            _mockCategoriaRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            _mockCategoriaRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _categoriaService.DeleteAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            _mockCategoriaRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _categoriaService.DeleteAsync(999);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Categoria não encontrada", result.Message);
        }

        [Fact]
        public async Task GetAllAsync_WhenRepositoryThrowsException_ShouldReturnInternalServerError()
        {
            // Arrange
            _mockCategoriaRepository.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _categoriaService.GetAllAsync();

            // Assert
            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Erro ao recuperar categorias", result.Message);
        }

        [Fact]
        public async Task CreateAsync_WhenRepositoryThrowsException_ShouldReturnInternalServerError()
        {
            // Arrange
            var createDto = new CategoryCreateRequest
            {
                Name = "Nova Categoria",
                Description = "Descrição"
            };

            _mockCategoriaRepository.Setup(r => r.NameExistsAsync(createDto.Name, null)).ReturnsAsync(false);
            _mockCategoriaRepository.Setup(r => r.CreateAsync(It.IsAny<Category>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _categoriaService.CreateAsync(createDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Erro ao criar categoria", result.Message);
        }
    }
}
