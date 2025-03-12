using System;

namespace DetectiveMysteryGamePlatform.Shared.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        Admin,
        Player
    }
} 