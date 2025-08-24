using Backend.Database;
using Backend.Database.Models;

namespace Backend.Tests.Seeds
{
    public class UserSeed
    {
        public List<User> Users { get; set; }

        public UserSeed(ApplicationDbContext context) { 
            Users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "João Silva",
                    Email = "joao@teste.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Created_At = DateTime.UtcNow.AddDays(-10),
                    Active = true
                },
                new User
                {
                    Id = 2,
                    Name = "Maria Santos",
                    Email = "maria@teste.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Created_At = DateTime.UtcNow.AddDays(-5),
                    Active = true
                },
                new User
                {
                    Id = 3,
                    Name = "Pedro Inativo",
                    Email = "pedro@teste.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Created_At = DateTime.UtcNow.AddDays(-3),
                    Active = false
                }
            };

            context.Users.AddRange(Users);
            context.SaveChanges();
        }
    }
}
