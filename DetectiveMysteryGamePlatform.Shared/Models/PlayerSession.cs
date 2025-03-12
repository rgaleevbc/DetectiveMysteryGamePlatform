using System;

namespace DetectiveMysteryGamePlatform.Shared.Models
{
    public class PlayerSession
    {
        public Guid Id { get; set; }
        public Guid GameSessionId { get; set; }
        public Guid? CharacterId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string InvitationToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime LastActiveAt { get; set; }
        public bool IsConnected { get; set; }
    }
} 