namespace issues.server.Infrastructure.Models.Stats
{
    public class ProjectStats
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public int OpenIssues { get; set; }
        public int ActiveIssues { get; set; }
        public int ClosedIssues { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
