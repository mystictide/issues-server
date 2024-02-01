namespace issues.server.Infrastructure.Models.Main
{
    public class Projects
    {
        public required int ID { get; set; }
        public required int CompanyID { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
