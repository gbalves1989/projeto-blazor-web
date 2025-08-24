using Backend.Database.Models;
using Backend.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace Backend.Tests.Services
{
    public class TestJwtService
    {
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;

        public TestJwtService()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"Jwt:Key", "MinhaChaveSecretaSuperSeguraParaJWT2024!@#$%"},
                {"Jwt:Issuer", "CategoriaUsuarioApi"},
                {"Jwt:Audience", "CategoriaUsuarioApiUsers"}
            });
            _configuration = configurationBuilder.Build();

            _jwtService = new JwtService(_configuration);
        }

        [Fact]
        public void GenerateToken_WithValidUser_ShouldReturnValidToken()
        {
            var user = new User
            {
                Id = 1,
                Name = "João Silva",
                Email = "joao@teste.com",
                Created_At = DateTime.UtcNow,
                Active = true
            };

            var token = _jwtService.GenerateToken(user);
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            Assert.True(tokenHandler.CanReadToken(token));

            var jwtToken = tokenHandler.ReadJwtToken(token);
            Assert.NotNull(jwtToken);
        }

        [Fact]
        public void GenerateToken_ShouldIncludeCorrectClaims()
        {
            var user = new User
            {
                Id = 1,
                Name = "João Silva",
                Email = "joao@teste.com",
                Created_At = DateTime.UtcNow,
                Active = true
            };

            var token = _jwtService.GenerateToken(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            Assert.NotNull(idClaim);
            Assert.Equal("1", idClaim.Value);

            var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            Assert.NotNull(nameClaim);
            Assert.Equal("João Silva", nameClaim.Value);

            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            Assert.NotNull(emailClaim);
            Assert.Equal("joao@teste.com", emailClaim.Value);
        }

        [Fact]
        public void GenerateToken_ShouldIncludeCorrectIssuerAndAudience()
        {
            var user = new User
            {
                Id = 1,
                Name = "João Silva",
                Email = "joao@teste.com",
                Created_At = DateTime.UtcNow,
                Active = true
            };

            var token = _jwtService.GenerateToken(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.Equal("CategoriaUsuarioApi", jwtToken.Issuer);
            Assert.Contains("CategoriaUsuarioApiUsers", jwtToken.Audiences);
        }

        [Fact]
        public void GenerateToken_ShouldHaveCorrectExpiration()
        {
            var user = new User
            {
                Id = 1,
                Name = "João Silva",
                Email = "joao@teste.com",
                Created_At = DateTime.UtcNow,
                Active = true
            };

            var beforeGeneration = DateTime.UtcNow;
            var token = _jwtService.GenerateToken(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var afterGeneration = DateTime.UtcNow;

            var expectedExpiration = beforeGeneration.AddHours(24);
            var actualExpiration = jwtToken.ValidTo;

            var timeDifference = Math.Abs((expectedExpiration - actualExpiration).TotalSeconds);
            Assert.True(timeDifference < 10, $"Diferença de tempo muito grande: {timeDifference} segundos");
        }

        [Fact]
        public void GenerateToken_WithNullUser_ShouldThrowException()
        {
            User user = null;
            Assert.Throws<ArgumentNullException>(() => _jwtService.GenerateToken(user));
        }

        [Fact]
        public void GenerateToken_WithDifferentUsers_ShouldGenerateDifferentTokens()
        {
            var user1 = new User
            {
                Id = 1,
                Name = "João Silva",
                Email = "joao@teste.com",
                Created_At = DateTime.UtcNow,
                Active = true
            };

            var user2 = new User
            {
                Id = 2,
                Name = "Maria Santos",
                Email = "maria@teste.com",
                Created_At = DateTime.UtcNow,
                Active = true
            };

            var token1 = _jwtService.GenerateToken(user1);
            var token2 = _jwtService.GenerateToken(user2);

            Assert.NotEqual(token1, token2);
        }

        [Fact]
        public void GenerateToken_SameUserMultipleTimes_ShouldGenerateDifferentTokens()
        {
            var user = new User
            {
                Id = 1,
                Name = "João Silva",
                Email = "joao@teste.com",
                Created_At = DateTime.UtcNow,
                Active = true
            };

            var token1 = _jwtService.GenerateToken(user);
            Thread.Sleep(1000); 
            var token2 = _jwtService.GenerateToken(user);

            Assert.NotEqual(token1, token2);
        }
    }
}
