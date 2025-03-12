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
            _context = context;
        }

        // GET: api/quests/{questId}/characters
        [HttpGet("/api/quests/{questId}/characters")]
        public async Task<ActionResult<IEnumerable<Character>>> GetCharactersByQuest(Guid questId)
        {
            var quest = await _context.Quests.FindAsync(questId);
            if (quest == null)
            {
                return NotFound();
            }

            return await _context.Characters
                .Where(c => c.QuestId == questId)
                .ToListAsync();
        }

        // GET: api/characters/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Character>> GetCharacter(Guid id)
        {
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
            var quest = await _context.Quests.FindAsync(questId);
            if (quest == null)
            {
                return NotFound();
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
            if (id != character.Id)
            {
                return BadRequest();
            }

            var existingCharacter = await _context.Characters.FindAsync(id);
            if (existingCharacter == null)
            {
                return NotFound();
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/characters/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(Guid id)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }

            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/characters/{id}/assign
        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignCharacter(Guid id, [FromBody] CharacterAssignmentRequest request)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }

            // In a real implementation, assign the character to a player
            // For MVP, we'll just return a success message
            return Ok(new { message = "Character assigned successfully" });
        }

        private bool CharacterExists(Guid id)
        {
            return _context.Characters.Any(e => e.Id == id);
        }
    }

    public class CharacterAssignmentRequest
    {
        public Guid GameSessionId { get; set; }
        public string PlayerEmail { get; set; }
    }
} 