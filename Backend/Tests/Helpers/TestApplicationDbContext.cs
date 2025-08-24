using Backend.Database;
using Backend.Tests.Seeds;
using Microsoft.EntityFrameworkCore;
using System;

namespace Backend.Tests.Helpers
{
    public class TestApplicationDbContext
    {
        public static ApplicationDbContext CreateInMemoryContext(string? databaseName = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();

            return context;
        }

        public static ApplicationDbContext CreateContextWithData(string? databaseName = null)
        {
            var context = CreateInMemoryContext(databaseName);
            UserSeed userSeed = new UserSeed(context);
            CategorySeed categorySeed = new CategorySeed(context);

            return context;
        }
    }
}
