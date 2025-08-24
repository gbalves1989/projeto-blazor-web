using Backend.Database;
using Backend.Database.Models;
using Backend.Database.Repositories;
using Backend.Tests.Helpers;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend.Tests.Repositories
{
    public class TestCategoryRepository
    {
        private readonly CategoryRepository _repository;
        private readonly ApplicationDbContext _context;

        public TestCategoryRepository()
        {
            _context = TestApplicationDbContext.CreateContextWithData();
            _repository = new CategoryRepository(_context);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnOnlyActiveCategories()
        {
            var result = await _repository.GetAllAsync();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); 
            Assert.All(result, c => Assert.True(c.Active));
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnCategory()
        {
            var categoryId = 1;
            var result = await _repository.GetByIdAsync(categoryId);

            Assert.NotNull(result);
            Assert.Equal(categoryId, result.Id);
            Assert.Equal("Tecnologia", result.Name);
            Assert.True(result.Active);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            var categoryId = 999;
            var result = await _repository.GetByIdAsync(categoryId);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_WithInactiveCategory_ShouldReturnNull()
        {
            var categoryId = 3; 
            var result = await _repository.GetByIdAsync(categoryId);
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_WithValidCategory_ShouldCreateCategory()
        {
            var newCategory = new Category
            {
                Name = "Nova Categoria",
                Description = "Descrição da nova categoria",
                Created_At = DateTime.UtcNow,
                Active = true
            };

            var result = await _repository.CreateAsync(newCategory);
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Nova Categoria", result.Name);
            Assert.Equal("Descrição da nova categoria", result.Description);

            var savedCategory = await _repository.GetByIdAsync(result.Id);
            Assert.NotNull(savedCategory);
        }

        [Fact]
        public async Task UpdateAsync_WithValidCategory_ShouldUpdateCategory()
        {
            var category = await _repository.GetByIdAsync(1);
            category.Name = "Tecnologia Atualizada";
            category.Description = "Descrição atualizada";

            var result = await _repository.UpdateAsync(category);
            Assert.NotNull(result);
            Assert.Equal("Tecnologia Atualizada", result.Name);
            Assert.Equal("Descrição atualizada", result.Description);

            var updatedCategory = await _repository.GetByIdAsync(1);
            Assert.Equal("Tecnologia Atualizada", updatedCategory.Name);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldSoftDeleteCategory()
        {
            var categoryId = 1;
            var result = await _repository.DeleteAsync(categoryId);
            Assert.True(result);

            var deletedCategory = _context.Categories.Find(categoryId);
            Assert.NotNull(deletedCategory);
            Assert.False(deletedCategory.Active);

            var categoryFromRepository = await _repository.GetByIdAsync(categoryId);
            Assert.Null(categoryFromRepository);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
        {
            var categoryId = 999;
            var result = await _repository.DeleteAsync(categoryId);
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
        {
            var categoryId = 1;
            var result = await _repository.ExistsAsync(categoryId);
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
        {
            var categoryId = 999;
            var result = await _repository.ExistsAsync(categoryId);
            Assert.False(result);
        }

        [Fact]
        public async Task NomeExistsAsync_WithExistingName_ShouldReturnTrue()
        {
            var nome = "Tecnologia";
            var result = await _repository.NameExistsAsync(nome);
            Assert.True(result);
        }

        [Fact]
        public async Task NomeExistsAsync_WithNonExistingName_ShouldReturnFalse()
        {
            var nome = "Categoria Inexistente";
            var result = await _repository.NameExistsAsync(nome);
            Assert.False(result);
        }

        [Fact]
        public async Task NomeExistsAsync_WithExcludeId_ShouldExcludeSpecifiedCategory()
        {
            var nome = "Tecnologia";
            var excludeId = 1; 
            var result = await _repository.NameExistsAsync(nome, excludeId);
            Assert.False(result); 
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnCategoriesOrderedByName()
        {
            var result = await _repository.GetAllAsync();
            Assert.NotNull(result);

            var categories = result.ToList();
            Assert.True(categories.Count >= 2);

            for (int i = 0; i < categories.Count - 1; i++)
            {
                Assert.True(string.Compare(
                    categories[i].Name, 
                    categories[i + 1].Name, 
                    StringComparison.OrdinalIgnoreCase) <= 0);
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
