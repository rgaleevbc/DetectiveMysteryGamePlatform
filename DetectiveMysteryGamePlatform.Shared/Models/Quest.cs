using System;

namespace DetectiveMysteryGamePlatform.Shared.Models
{
    public class Quest
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public int NumberOfRounds { get; set; }
    }
} 