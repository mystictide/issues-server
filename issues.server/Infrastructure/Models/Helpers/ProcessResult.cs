namespace issues.server.Infrasructure.Models.Users.Helpers
{
    public class ProcessResult
    {
        public ProcessState State { get; set; }
        public string? Message { get; set; }
        public int ReturnID { get; set; }

    }
    public enum ProcessState
    {
        Success = 1,
        Error = 2
    }
}
