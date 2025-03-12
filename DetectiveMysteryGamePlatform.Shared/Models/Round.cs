using System;

namespace DetectiveMysteryGamePlatform.Shared.Models
{
    public class Round
    {
        public Guid Id { get; set; }
        public Guid QuestId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
} 