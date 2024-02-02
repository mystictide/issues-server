namespace issues.server.Infrastructure.Models.Main
{
    public class Issues
    {
        public required int ID { get; set; }
        public required Projects Project { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required Types Type { get; set; }
        public required States Status { get; set; }
        public required Priorities Priority { get; set; }
        public required Users CreatedBy { get; set; }
        public required List<Users> AssignedTo { get; set; }
        public required DateTime CreatedDate { get; set; }
        public required DateTime EndDate { get; set; }
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
