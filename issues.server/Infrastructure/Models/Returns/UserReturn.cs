
namespace issues.server.Infrasructure.Models.Returns
{
    public class UserReturn
    {
        public int? UID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public int? AuthType { get; set; }
        public string? Token { get; set; }
    }
}
