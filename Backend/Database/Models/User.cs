using System.ComponentModel.DataAnnotations;

namespace Backend.Database.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public DateTime Created_At { get; set; } = DateTime.UtcNow;

        public bool Active { get; set; } = true;
    }
}
