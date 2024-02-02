namespace issues.server.Infrastructure.Models.User
{
    public class Roles
    {
        public required int ID { get; set; }
        public required int CompanyID { get; set; }
        public required string Name { get; set; }
        public required int[] Attributes { get; set; }

        public static readonly Dictionary<int, string> RoleAttributes = new()
        {
            { 1, "Has full access to all features and data." },
            { 2, "Can manage users, projects and system settings." },
            { 3, "Can assign users to projects and issues." },
            { 4, "Can create, edit and delete projects and issues." },
            { 5, "Can assign users to issues." },
            { 6, "Can create, edit and close issues." },
            { 7, "Can view all projects." },
            { 8, "Can view all issues." },
            { 9, "Can view all users." },
        };
    }
}
