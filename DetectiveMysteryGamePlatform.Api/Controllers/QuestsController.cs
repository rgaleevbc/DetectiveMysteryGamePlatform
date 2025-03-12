using DetectiveMysteryGamePlatform.Api.Data;
using DetectiveMysteryGamePlatform.Api.Services;
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
    public class QuestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PdfProcessingService _pdfProcessingService;

        public QuestsController(ApplicationDbContext context, PdfProcessingService pdfProcessingService)
        {
            _context = context;
            _pdfProcessingService = pdfProcessingService;
        }

        // GET: api/quests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quest>>> GetQuests()
        {
            return await _context.Quests.ToListAsync();
        }

        // GET: api/quests/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Quest>> GetQuest(Guid id)
        {
            var quest = await _context.Quests.FindAsync(id);

            if (quest == null)
            {
                return NotFound();
            }

            return quest;
        }

        // POST: api/quests
        [HttpPost]
        public async Task<ActionResult<Quest>> CreateQuest(Quest quest)
        {
            quest.Id = Guid.NewGuid();
            quest.CreatedAt = DateTime.UtcNow;
            quest.UpdatedAt = DateTime.UtcNow;
            // In a real implementation, get the user ID from the claims
            // For MVP, we'll just use a placeholder
            quest.CreatedById = Guid.Empty;

            _context.Quests.Add(quest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuest), new { id = quest.Id }, quest);
        }

        // PUT: api/quests/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuest(Guid id, Quest quest)
        {
            if (id != quest.Id)
            {
                return BadRequest();
            }

            var existingQuest = await _context.Quests.FindAsync(id);
            if (existingQuest == null)
            {
                return NotFound();
            }

            existingQuest.Title = quest.Title;
            existingQuest.Description = quest.Description;
            existingQuest.NumberOfRounds = quest.NumberOfRounds;
            existingQuest.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestExists(id))
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

        // DELETE: api/quests/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuest(Guid id)
        {
            var quest = await _context.Quests.FindAsync(id);
            if (quest == null)
            {
                return NotFound();
            }

            _context.Quests.Remove(quest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/quests/{id}/upload-pdf
        [HttpPost("{id}/upload-pdf")]
        public async Task<IActionResult> UploadPdf(Guid id, [FromForm] PdfUploadRequest request)
        {
            var quest = await _context.Quests.FindAsync(id);
            if (quest == null)
            {
                return NotFound();
            }

            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            // Save the PDF file
            var filePath = await _pdfProcessingService.SavePdfFile(request.File, id);

            return Ok(new { message = "PDF uploaded successfully", filePath });
        }

        // POST: api/quests/{id}/extract-content
        [HttpPost("{id}/extract-content")]
        public async Task<IActionResult> ExtractContent(Guid id, [FromBody] ContentExtractionRequest request)
        {
            var quest = await _context.Quests.FindAsync(id);
            if (quest == null)
            {
                return NotFound();
            }

            // In a real implementation, extract content from the PDF
            // For MVP, we'll create placeholder content
            foreach (var item in request.Items)
            {
                var contentId = Guid.NewGuid();
                var content = new Content
                {
                    Id = contentId,
                    QuestId = id,
                    RoundId = item.RoundId,
                    CharacterId = item.CharacterId,
                    Type = (ContentType)Enum.Parse(typeof(ContentType), item.Type),
                    Title = $"Content from page {item.PageNumber}",
                    IsPublic = item.IsPublic,
                    DisplayOrder = 0
                };

                // Extract image from PDF (placeholder for MVP)
                content.ImagePath = await _pdfProcessingService.ExtractImageFromPdf("placeholder.pdf", item.PageNumber, contentId);

                _context.Contents.Add(content);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Content extracted successfully" });
        }

        private bool QuestExists(Guid id)
        {
            return _context.Quests.Any(e => e.Id == id);
        }
    }

    public class PdfUploadRequest
    {
        public Microsoft.AspNetCore.Http.IFormFile File { get; set; }
    }

    public class ContentExtractionRequest
    {
        public List<ContentExtractionItem> Items { get; set; }
    }

    public class ContentExtractionItem
    {
        public string Type { get; set; }
        public int PageNumber { get; set; }
        public bool IsPublic { get; set; }
        public Guid? RoundId { get; set; }
        public Guid? CharacterId { get; set; }
    }
} 