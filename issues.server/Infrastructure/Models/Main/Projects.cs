namespace issues.server.Infrastructure.Models.Main
{
    public class Projects
    {
        public int ID { get; set; }
        public int CompanyID { get; set; }
        public int AssignedTo { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
