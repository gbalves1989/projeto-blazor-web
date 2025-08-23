using System.Text.Json.Serialization;

namespace Backend.Dtos.Responses.Category
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [JsonIgnore]
        public DateTime Created_At { get; set; }
        public bool Active { get; set; }
    }
}
