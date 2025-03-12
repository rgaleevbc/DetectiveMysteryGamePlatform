using System;

namespace DetectiveMysteryGamePlatform.Shared.Models
{
    public class Character
    {
        public Guid Id { get; set; }
        public Guid QuestId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPublicInfo { get; set; }
        public string? AvatarImagePath { get; set; }
    }
} 