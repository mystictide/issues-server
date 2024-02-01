using issues.server.Infrastructure.Models.Main;

namespace issues.server.Infrastructure.Models.Issues
{
    public class Comments
    {
        public required int ID { get; set; }
        public required int IssueID { get; set; }
        public required Users User { get; set; }
        public required string Body{ get; set; }
        public required DateTime CreatedDate{ get; set; }
    }
}
