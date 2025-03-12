using System;

namespace DetectiveMysteryGamePlatform.Shared.Models
{
    public class Content
    {
        public Guid Id { get; set; }
        public Guid QuestId { get; set; }
        public Guid? RoundId { get; set; }
        public Guid? CharacterId { get; set; }
        public ContentType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public int DisplayOrder { get; set; }
    }

    public enum ContentType
    {
        GeneralInstruction,
        CharacterInstruction,
        Clue,
        RoundDescription
    }
} 