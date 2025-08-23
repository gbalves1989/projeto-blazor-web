using System.ComponentModel.DataAnnotations;

namespace Backend.Database.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime Created_At { get; set; } = DateTime.UtcNow;

        public bool Active { get; set; } = true;
    }
}
