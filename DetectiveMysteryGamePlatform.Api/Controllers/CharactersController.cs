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
    public class CharactersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CharactersController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/quests/{questId}/characters
        [HttpGet("/api/quests/{questId}/characters")]
        public async Task<ActionResult<IEnumerable<Character>>> GetCharactersByQuest(Guid questId)
        {
            if (questId == Guid.Empty)
            {
                return BadRequest("Invalid quest ID");
            }

            var quest = await _context.Quests.FindAsync(questId);
            if (quest == null)
            {
                return NotFound();
            }

            var characters = await _context.Characters
                .Where(c => c.QuestId == questId)
                .ToListAsync();

            return characters;
        }

        // GET: api/characters/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Character>> GetCharacter(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid character ID");
            }

            var character = await _context.Characters.FindAsync(id);

            if (character == null)
            {
                return NotFound();
            }

            return character;
        }

        // POST: api/quests/{questId}/characters
        [HttpPost("/api/quests/{questId}/characters")]
        public async Task<ActionResult<Character>> CreateCharacter(Guid questId, Character character)
        {
            if (questId == Guid.Empty)
            {
                return BadRequest("Invalid quest ID");
            }

            if (character == null)
            {
                return BadRequest("Character data is required");
            }

            var quest = await _context.Quests.FindAsync(questId);
            if (quest == null)
            {
                return NotFound("Quest not found");
            }

            character.Id = Guid.NewGuid();
            character.QuestId = questId;

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCharacter), new { id = character.Id }, character);
        }

        // PUT: api/characters/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCharacter(Guid id, Character character)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid character ID");
            }

            if (character == null)
            {
                return BadRequest("Character data is required");
            }

            if (id != character.Id)
            {
                return BadRequest("Character ID mismatch");
            }

            var existingCharacter = await _context.Characters.FindAsync(id);
            if (existingCharacter == null)
            {
                return NotFound("Character not found");
            }

            existingCharacter.Name = character.Name;
            existingCharacter.Description = character.Description;
            existingCharacter.IsPublicInfo = character.IsPublicInfo;
            existingCharacter.AvatarImagePath = character.AvatarImagePath;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CharacterExists(id))
                {
                    return NotFound("Character no longer exists");
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/characters/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid character ID");
            }

            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound("Character not found");
            }

            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/characters/{id}/assign
        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignCharacter(Guid id, [FromBody] CharacterAssignmentRequest request)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid character ID");
            }

            if (request == null)
            {
                return BadRequest("Assignment request data is required");
            }

            if (request.GameSessionId == Guid.Empty)
            {
                return BadRequest("Invalid game session ID");
            }

            if (string.IsNullOrWhiteSpace(request.PlayerEmail))
            {
                return BadRequest("Player email is required");
            }

            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound("Character not found");
            }

            return Ok(new { message = "Character assigned successfully" });
        }

        private bool CharacterExists(Guid id)
        {
            return id != Guid.Empty && _context.Characters.Any(e => e.Id == id);
        }
    }

    public class CharacterAssignmentRequest
    {
        public Guid GameSessionId { get; set; }
        public string PlayerEmail { get; set; } = string.Empty;
    }
} 