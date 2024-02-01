using Newtonsoft.Json;
using issues.server.Infrastructure.Models.User;

namespace issues.server.Infrastructure.Models.Main
{
    public class Users
    {
        public required int ID { get; set; }
        public required Companies Company { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        [JsonIgnore]
        public required string Password { get; set; }
        public required Roles Role { get; set; }
        public string? Token { get; set; }
    }
}