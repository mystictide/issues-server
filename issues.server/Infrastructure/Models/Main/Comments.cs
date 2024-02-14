using issues.server.Infrastructure.Models.Response;

namespace issues.server.Infrastructure.Models.Main
{
    public class Comments
    {
        public int ID { get; set; }
        public int IssueID { get; set; }
        public UserResponse? User { get; set; }
        public string? Body { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
