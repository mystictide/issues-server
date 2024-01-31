using Dapper.Contrib.Extensions;

namespace issues.server.Infrasructure.Models.Users
{
    [Table("users")]
    public class Users
    {
        [Key]
        public int? ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? AuthType { get; set; }

        [Write(false)]
        public string? Token { get; set; }
    }
}
