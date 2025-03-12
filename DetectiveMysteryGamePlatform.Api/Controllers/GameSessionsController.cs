using DetectiveMysteryGamePlatform.Api.Data;
using DetectiveMysteryGamePlatform.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DetectiveMysteryGamePlatform.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class GameSessionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GameSessionsController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/game-sessions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameSession>>> GetGameSessions()
        {
            var sessions = await _context.GameSessions.ToListAsync();
            return sessions;
        }

        // GET: api/game-sessions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GameSession>> GetGameSession(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid game session ID");
            }

            var gameSession = await _context.GameSessions.FindAsync(id);

            if (gameSession == null)
            {
                return NotFound("Game session not found");
            }

            return gameSession;
        }

        // POST: api/game-sessions
        [HttpPost]
        public async Task<ActionResult<GameSession>> CreateGameSession(GameSessionCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request data is required");
            }

            if (request.QuestId == Guid.Empty)
            {
                return BadRequest("Invalid quest ID");
            }

            var quest = await _context.Quests.FindAsync(request.QuestId);
            if (quest == null)
            {
                return NotFound("Quest not found");
            }

            // Get the first round of the quest
            var firstRound = await _context.Rounds
                .Where(r => r.QuestId == request.QuestId)
                .OrderBy(r => r.Number)
                .FirstOrDefaultAsync();

            if (firstRound == null)
            {
                return BadRequest("Quest has no rounds");
            }

            var gameSession = new GameSession
            {
                Id = Guid.NewGuid(),
                QuestId = request.QuestId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = GameSessionStatus.Created,
                CurrentRoundId = firstRound.Id,
                CurrentRound = firstRound.Number,
                PlayerSessions = new List<PlayerSession>()
            };

            _context.GameSessions.Add(gameSession);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGameSession), new { id = gameSession.Id }, gameSession);
        }

        // PUT: api/game-sessions/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateGameSessionStatus(Guid id, [FromBody] GameSessionStatusUpdateRequest request)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid game session ID");
            }

            if (request == null)
            {
                return BadRequest("Status update data is required");
            }

            var gameSession = await _context.GameSessions.FindAsync(id);
            if (gameSession == null)
            {
                return NotFound("Game session not found");
            }

            gameSession.Status = request.Status;
            gameSession.UpdatedAt = DateTime.UtcNow;

            if (request.Status == GameSessionStatus.InProgress && !gameSession.StartedAt.HasValue)
            {
                gameSession.StartedAt = DateTime.UtcNow;
            }
            else if (request.Status == GameSessionStatus.Completed && !gameSession.EndedAt.HasValue)
            {
                gameSession.EndedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/game-sessions/{id}/invite-player
        [HttpPost("{id}/invite-player")]
        public async Task<IActionResult> InvitePlayer(Guid id, [FromBody] PlayerInviteRequest request)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid game session ID");
            }

            if (request == null)
            {
                return BadRequest("Player invite data is required");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest("Player email is required");
            }

            var gameSession = await _context.GameSessions.FindAsync(id);
            if (gameSession == null)
            {
                return NotFound("Game session not found");
            }

            // Check if player is already invited
            var existingInvitation = await _context.PlayerSessions
                .FirstOrDefaultAsync(ps => ps.GameSessionId == id && ps.Email == request.Email);

            if (existingInvitation != null)
            {
                return BadRequest("Player already invited");
            }

            var invitationToken = Guid.NewGuid().ToString();

            var playerSession = new PlayerSession
            {
                Id = Guid.NewGuid(),
                GameSessionId = id,
                CharacterId = null, // Will be assigned later
                PlayerName = string.Empty, // Will be provided by player
                InvitationToken = invitationToken,
                Email = request.Email,
                LastActiveAt = DateTime.UtcNow,
                IsConnected = false
            };

            _context.PlayerSessions.Add(playerSession);
            await _context.SaveChangesAsync();

            // In a real implementation, send an email with the invitation link
            // For MVP, we'll just return the token
            return Ok(new { invitationToken });
        }

        // PUT: api/game-sessions/{id}/advance-round
        [HttpPut("{id}/advance-round")]
        public async Task<IActionResult> AdvanceRound(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid game session ID");
            }

            var gameSession = await _context.GameSessions.FindAsync(id);
            if (gameSession == null)
            {
                return NotFound("Game session not found");
            }

            // Get current round
            var currentRound = await _context.Rounds.FindAsync(gameSession.CurrentRoundId);
            if (currentRound == null)
            {
                return BadRequest("Current round not found");
            }

            // Get next round
            var nextRound = await _context.Rounds
                .Where(r => r.QuestId == gameSession.QuestId && r.Number > currentRound.Number)
                .OrderBy(r => r.Number)
                .FirstOrDefaultAsync();

            if (nextRound == null)
            {
                // No more rounds, complete the game
                gameSession.Status = GameSessionStatus.Completed;
                gameSession.EndedAt = DateTime.UtcNow;
            }
            else
            {
                gameSession.CurrentRoundId = nextRound.Id;
                gameSession.CurrentRound = nextRound.Number;
                gameSession.Status = GameSessionStatus.InProgress;
            }

            gameSession.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class GameSessionCreateRequest
    {
        public Guid QuestId { get; set; }
    }

    public class GameSessionStatusUpdateRequest
    {
        public GameSessionStatus Status { get; set; }
    }

    public class PlayerInviteRequest
    {
        public string Email { get; set; } = string.Empty;
    }
} 