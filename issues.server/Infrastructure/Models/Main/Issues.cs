using issues.server.Infrastructure.Models.Response;

namespace issues.server.Infrastructure.Models.Main
{
    public class Issues
    {
        public int ID { get; set; }
        public Projects? Project { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Types Type { get; set; }
        public States Status { get; set; }
        public Priorities Priority { get; set; }
        public UserResponse? CreatedBy { get; set; }
        public List<UserResponse>? AssignedTo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
    public enum States
    {
        Open = 1,
        InProgress = 2,
        Closed = 3
    }
    public enum Types
    {
        Bug = 1,
        Feature = 2,
        Enhancement = 3,
        Task = 4
    }
    public enum Priorities
    {
        High = 1,
        Medium = 2,
        Low = 3
    }
}
