using Backend.Database;
using Backend.Database.Models;

namespace Backend.Tests.Seeds
{
    public class CategorySeed
    {
        public List<Category> Categories { get; set; }

        public CategorySeed(ApplicationDbContext context) 
        {
            Categories = new List<Category>
            {
                new Category
                {
                    Id = 1,
                    Name = "Tecnologia",
                    Description = "Categoria para assuntos relacionados à tecnologia",
                    Created_At = DateTime.UtcNow.AddDays(-8),
                    Active = true
                },
                new Category
                {
                    Id = 2,
                    Name = "Esportes",
                    Description = "Categoria para assuntos relacionados a esportes",
                    Created_At = DateTime.UtcNow.AddDays(-6),
                    Active = true
                },
                new Category
                {
                    Id = 3,
                    Name = "Categoria Inativa",
                    Description = "Categoria desativada para testes",
                    Created_At = DateTime.UtcNow.AddDays(-2),
                    Active = false
                }
            };

            context.Categories.AddRange(Categories);
            context.SaveChanges();
        }
    }
}
