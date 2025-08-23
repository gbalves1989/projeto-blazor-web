using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos.Requests.Category
{
    public class CategoryUpdateRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string? Description { get; set; }

        public bool Active { get; set; } = true;
    }
}
