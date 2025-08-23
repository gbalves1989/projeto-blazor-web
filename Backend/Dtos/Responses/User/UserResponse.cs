using System.Text.Json.Serialization;

namespace Backend.Dtos.Responses.User
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [JsonIgnore]
        public DateTime Created_At { get; set; }

        public bool Active { get; set; }
    }
}
