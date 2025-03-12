using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DetectiveMysteryGamePlatform.Shared.Models
{
    public class GameSession
    {
        public Guid Id { get; set; }
        public Guid QuestId { get; set; }
        public GameSessionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CurrentRound { get; set; }
        public Guid CurrentRoundId { get; set; }
        public List<PlayerSession> PlayerSessions { get; set; } = new List<PlayerSession>();
    }

    public class GameSessionResponse
    {
        public Guid Id { get; set; }
        public Guid QuestId { get; set; }
        public string? QuestTitle { get; set; }
        public GameSessionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int CurrentRound { get; set; }
        public int MaxPlayers { get; set; }
        public string? InviteToken { get; set; }
        public bool IsActive => Status == GameSessionStatus.InProgress || Status == GameSessionStatus.Paused;
        public int CurrentRoundNumber => CurrentRound;
        public List<PlayerSessionResponse> Players { get; set; } = new List<PlayerSessionResponse>();
    }

    public class PlayerSessionResponse
    {
        public Guid Id { get; set; }
        public string? PlayerName { get; set; }
        public string? Email { get; set; }
        public Guid? CharacterId { get; set; }
        public string? CharacterName { get; set; }
        public bool IsConnected { get; set; }
        public Guid? AssignedCharacterId => CharacterId;
    }

    public enum GameSessionStatus
    {
        Created,
        InProgress,
        Paused,
        Completed
    }
}