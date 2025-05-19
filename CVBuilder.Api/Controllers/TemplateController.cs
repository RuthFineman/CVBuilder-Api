using Amazon.S3.Model;
using Amazon.S3;
using CVBuilder.Core.Models;
using CVBuilder.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Org.BouncyCastle.Math.EC.ECCurve;
using CVBuilder.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CVBuilder.Api.Controllers
{
    //אולי להוסיף רק למנהל
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplateController(ITemplateService templateService)
        {
            _templateService = templateService;
        }
        //העלאת תבנית
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // קריאה לפונקציה שמעלה את הקובץ ל-S3 ושומרת את פרטי התבנית
            var fileUrl = await _templateService.AddTemplateAsync(file, file.FileName);
            return Ok(new { FileUrl = fileUrl });
        }
        //מחיקת תבנית
        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteTemplateAsync(string fileName)
        {
            try
            {
                var result = await _templateService.DeleteTemplateAsync(fileName);
                if (result)
                {
                    return Ok("The file was deleted successfully.");
                }
                else
                {
                    return NotFound("File not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetTemplates()
        {
            var files = await _templateService.GetAllTamplatesAsync();
            return Ok(files);
        }
        //לקבלת URL של תבנית בודדה
        [HttpGet("{index}")]
        public async Task<IActionResult> GetFile(int index)
        {
            try
            {
                var fileUrl = await _templateService.GetFileAsync(index);

                if (string.IsNullOrEmpty(fileUrl))
                    return NotFound("File not found");

                return Ok(fileUrl);
            }
            catch (Exception ex)
            {
                // לוג שגיאה - מומלץ להוסיף לוג פה
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPatch("{id}/status")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateTemplateStatus(int id, [FromBody] bool inUse)
        {
            var updatedTemplate = await _templateService.UpdateTemplateStatusAsync(id, inUse);
            if (updatedTemplate == null)
                return NotFound("Template not found.");

            return Ok(updatedTemplate);
        }
        //מקבל קישור ומחזיר מזהה
        [HttpGet("get-id-by-url")]
        public async Task<IActionResult> GetTemplateIdByUrl([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
                return BadRequest("URL is required");

            var id = await _templateService.GetTemplateIdByUrlAsync(url);

            if (id == null)
                return NotFound("Template not found");

            return Ok(id);
        }
    }
}
