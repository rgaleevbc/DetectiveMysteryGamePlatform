using DetectiveMysteryGamePlatform.Api.Data;
using DetectiveMysteryGamePlatform.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DetectiveMysteryGamePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlayerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/player/{token}/game-info
        [HttpGet("{token}/game-info")]
        public async Task<ActionResult<GameInfoResponse>> GetGameInfo(string token)
        {
            var playerSession = await _context.PlayerSessions
                .FirstOrDefaultAsync(ps => ps.InvitationToken == token);

            if (playerSession == null)
            {
                return NotFound();
            }

            var gameSession = await _context.GameSessions.FindAsync(playerSession.GameSessionId);
            if (gameSession == null)
            {
                return NotFound();
            }

            var quest = await _context.Quests.FindAsync(gameSession.QuestId);
            if (quest == null)
            {
                return NotFound();
            }

            var currentRound = await _context.Rounds.FindAsync(gameSession.CurrentRoundId);
            if (currentRound == null)
            {
                return NotFound();
            }

            return new GameInfoResponse
            {
                GameSessionId = gameSession.Id,
                QuestTitle = quest.Title,
                QuestDescription = quest.Description,
                GameSessionStatus = gameSession.Status,
                CurrentRoundNumber = currentRound.Number,
                CurrentRoundTitle = currentRound.Title,
                PlayerName = playerSession.PlayerName,
                IsConnected = playerSession.IsConnected,
                HasCharacterAssigned = playerSession.CharacterId.HasValue
            };
        }

        // GET: api/player/{token}/character
        [HttpGet("{token}/character")]
        public async Task<ActionResult<CharacterResponse>> GetCharacter(string token)
        {
            var playerSession = await _context.PlayerSessions
                .FirstOrDefaultAsync(ps => ps.InvitationToken == token);

            if (playerSession == null)
            {
                return NotFound();
            }

            if (!playerSession.CharacterId.HasValue)
            {
                return BadRequest("No character assigned");
            }

            var character = await _context.Characters.FindAsync(playerSession.CharacterId.Value);
            if (character == null)
            {
                return NotFound();
            }

            return new CharacterResponse
            {
                Id = character.Id,
                Name = character.Name,
                Description = character.Description,
                AvatarImagePath = character.AvatarImagePath
            };
        }

        // GET: api/player/{token}/public-characters
        [HttpGet("{token}/public-characters")]
        public async Task<ActionResult<IEnumerable<CharacterResponse>>> GetPublicCharacters(string token)
        {
            var playerSession = await _context.PlayerSessions
                .FirstOrDefaultAsync(ps => ps.InvitationToken == token);

            if (playerSession == null)
            {
                return NotFound();
            }

            var gameSession = await _context.GameSessions.FindAsync(playerSession.GameSessionId);
            if (gameSession == null)
            {
                return NotFound();
            }

            var publicCharacters = await _context.Characters
                .Where(c => c.QuestId == gameSession.QuestId && c.IsPublicInfo)
                .Select(c => new CharacterResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    AvatarImagePath = c.AvatarImagePath
                })
                .ToListAsync();

            return publicCharacters;
        }

        // GET: api/player/{token}/current-round
        [HttpGet("{token}/current-round")]
        public async Task<ActionResult<RoundResponse>> GetCurrentRound(string token)
        {
            var playerSession = await _context.PlayerSessions
                .FirstOrDefaultAsync(ps => ps.InvitationToken == token);

            if (playerSession == null)
            {
                return NotFound();
            }

            var gameSession = await _context.GameSessions.FindAsync(playerSession.GameSessionId);
            if (gameSession == null)
            {
                return NotFound();
            }

            var round = await _context.Rounds.FindAsync(gameSession.CurrentRoundId);
            if (round == null)
            {
                return NotFound();
            }

            // Get public content for this round
            var publicContent = await _context.Contents
                .Where(c => c.RoundId == round.Id && c.IsPublic)
                .ToListAsync();

            // Get character-specific content if character is assigned
            var characterContent = new List<Content>();
            if (playerSession.CharacterId.HasValue)
            {
                characterContent = await _context.Contents
                    .Where(c => c.RoundId == round.Id && c.CharacterId == playerSession.CharacterId.Value)
                    .ToListAsync();
            }

            return new RoundResponse
            {
                RoundNumber = round.Number,
                Title = round.Title,
                Description = round.Description,
                PublicContent = publicContent.Select(c => new ContentResponse
                {
                    Id = c.Id,
                    Title = c.Title,
                    Type = c.Type,
                    ImagePath = c.ImagePath
                }).ToList(),
                CharacterContent = characterContent.Select(c => new ContentResponse
                {
                    Id = c.Id,
                    Title = c.Title,
                    Type = c.Type,
                    ImagePath = c.ImagePath
                }).ToList()
            };
        }

        // GET: api/player/{token}/revealed-content
        [HttpGet("{token}/revealed-content")]
        public async Task<ActionResult<IEnumerable<ContentResponse>>> GetRevealedContent(string token)
        {
            var playerSession = await _context.PlayerSessions
                .FirstOrDefaultAsync(ps => ps.InvitationToken == token);

            if (playerSession == null)
            {
                return NotFound();
            }

            var gameSession = await _context.GameSessions.FindAsync(playerSession.GameSessionId);
            if (gameSession == null)
            {
                return NotFound();
            }

            // Get all rounds up to the current one
            var currentRound = await _context.Rounds.FindAsync(gameSession.CurrentRoundId);
            if (currentRound == null)
            {
                return NotFound();
            }

            var previousRounds = await _context.Rounds
                .Where(r => r.QuestId == gameSession.QuestId && r.Number < currentRound.Number)
                .ToListAsync();

            var roundIds = previousRounds.Select(r => r.Id).ToList();
            roundIds.Add(currentRound.Id);

            // Get all public content for these rounds
            var publicContent = await _context.Contents
                .Where(c => roundIds.Contains(c.RoundId.Value) && c.IsPublic)
                .ToListAsync();

            // Get character-specific content if character is assigned
            var characterContent = new List<Content>();
            if (playerSession.CharacterId.HasValue)
            {
                characterContent = await _context.Contents
                    .Where(c => roundIds.Contains(c.RoundId.Value) && c.CharacterId == playerSession.CharacterId.Value)
                    .ToListAsync();
            }

            var allContent = publicContent.Concat(characterContent)
                .Select(c => new ContentResponse
                {
                    Id = c.Id,
                    Title = c.Title,
                    Type = c.Type,
                    ImagePath = c.ImagePath
                })
                .ToList();

            return allContent;
        }

        // POST: api/player/{token}/join
        [HttpPost("{token}/join")]
        public async Task<IActionResult> JoinGame(string token, [FromBody] JoinGameRequest request)
        {
            var playerSession = await _context.PlayerSessions
                .FirstOrDefaultAsync(ps => ps.InvitationToken == token);

            if (playerSession == null)
            {
                return NotFound();
            }

            playerSession.PlayerName = request.PlayerName;
            playerSession.IsConnected = true;
            playerSession.LastActiveAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Joined game successfully" });
        }
    }

    public class GameInfoResponse
    {
        public Guid GameSessionId { get; set; }
        public string QuestTitle { get; set; }
        public string QuestDescription { get; set; }
        public GameSessionStatus GameSessionStatus { get; set; }
        public int CurrentRoundNumber { get; set; }
        public string CurrentRoundTitle { get; set; }
        public string PlayerName { get; set; }
        public bool IsConnected { get; set; }
        public bool HasCharacterAssigned { get; set; }
    }

    public class CharacterResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AvatarImagePath { get; set; }
    }

    public class RoundResponse
    {
        public int RoundNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<ContentResponse> PublicContent { get; set; }
        public List<ContentResponse> CharacterContent { get; set; }
    }

    public class ContentResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public ContentType Type { get; set; }
        public string ImagePath { get; set; }
    }

    public class JoinGameRequest
    {
        public string PlayerName { get; set; }
    }
} 