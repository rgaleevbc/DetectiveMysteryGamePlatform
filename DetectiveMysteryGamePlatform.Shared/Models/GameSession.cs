using System;

namespace DetectiveMysteryGamePlatform.Shared.Models
{
    public class GameSession
    {
        public Guid Id { get; set; }
        public Guid QuestId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public GameSessionStatus Status { get; set; }
        public Guid CurrentRoundId { get; set; }
    }

    public enum GameSessionStatus
    {
        Created,
        InProgress,
        Paused,
        Completed
    }
} 