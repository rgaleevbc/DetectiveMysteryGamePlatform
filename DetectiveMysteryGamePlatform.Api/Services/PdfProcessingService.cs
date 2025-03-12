using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DetectiveMysteryGamePlatform.Api.Services
{
    public class PdfProcessingService
    {
        private readonly string _uploadDirectory;

        public PdfProcessingService(string uploadDirectory)
        {
            _uploadDirectory = uploadDirectory;
            
            // Ensure the upload directory exists
            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }
        }

        public async Task<string> SavePdfFile(IFormFile file, Guid questId)
        {
            // Create a directory for this quest
            var questDirectory = Path.Combine(_uploadDirectory, questId.ToString());
            if (!Directory.Exists(questDirectory))
            {
                Directory.CreateDirectory(questDirectory);
            }

            // Generate a unique filename
            var fileName = $"{Guid.NewGuid()}.pdf";
            var filePath = Path.Combine(questDirectory, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        public async Task<string> ExtractImageFromPdf(string pdfPath, int pageNumber, Guid contentId)
        {
            // In a real implementation, use a PDF library like iTextSharp or PDFsharp
            // to extract images from the PDF
            
            // For MVP, we'll just return a placeholder
            var questId = Path.GetFileName(Path.GetDirectoryName(pdfPath));
            var contentDirectory = Path.Combine(_uploadDirectory, questId, "content");
            if (!Directory.Exists(contentDirectory))
            {
                Directory.CreateDirectory(contentDirectory);
            }

            var imagePath = Path.Combine(contentDirectory, $"{contentId}.png");
            
            // In a real implementation, extract the image from the PDF and save it
            // For MVP, we'll just create an empty file
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                // Empty file
            }

            // Return the relative path to the image
            return $"/uploads/{questId}/content/{contentId}.png";
        }
    }
} 