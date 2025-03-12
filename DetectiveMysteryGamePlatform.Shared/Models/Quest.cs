using System;
using System.ComponentModel.DataAnnotations;

namespace DetectiveMysteryGamePlatform.Shared.Models
{
    public class Quest
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters")]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public Guid CreatedById { get; set; }

        [Required(ErrorMessage = "Number of rounds is required")]
        [Range(1, 20, ErrorMessage = "Number of rounds must be between 1 and 20")]
        public int NumberOfRounds { get; set; }
    }
} 