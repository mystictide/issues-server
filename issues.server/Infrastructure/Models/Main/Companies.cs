using Newtonsoft.Json;

namespace issues.server.Infrastructure.Models.Main
{
    public class Companies
    {
        public required int ID { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        [JsonIgnore]
        public required string Password { get; set; }
        public string? Token { get; set; }
    }
}
