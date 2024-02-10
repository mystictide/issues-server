namespace issues.server.Infrastructure.Models.Main
{
    public class Users
    {
        public int ID { get; set; }
        public Companies? Company { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Roles? Role { get; set; }
        public string? Token { get; set; }
        public bool IsActive { get; set; }
        public string? Name
        {
            get { return FirstName + " " + LastName; }
            set { }
        }
    }
}