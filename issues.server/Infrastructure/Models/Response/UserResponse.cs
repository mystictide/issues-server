﻿using issues.server.Infrastructure.Models.Main;

namespace issues.server.Infrastructure.Models.Response
{
    public class UserResponse
    {
        public int ID { get; set; }
        public Companies? Company { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public Roles? Role { get; set; }
        public string? Token { get; set; }
        public bool IsActive { get; set; }
    }
}
