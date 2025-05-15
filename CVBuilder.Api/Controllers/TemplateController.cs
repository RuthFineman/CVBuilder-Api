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
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetTemplates()
        {
            var files = await _templateService.GetAllTamplatesAsync();
            return Ok(files);

        }
        [HttpGet("{index}")]
        public async Task<IActionResult> GetFile(int index)
        {
            var fileUrl = await _templateService.GetFileAsync(index);

            if (string.IsNullOrEmpty(fileUrl))
                return NotFound("File not found");

            return Ok(fileUrl);
        }
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateTemplateStatus(int id, [FromBody] Template template)
        {
            var updatedTemplate = await _templateService.UpdateTemplateStatusAsync(id, template.InUse);
            if (updatedTemplate == null)
                return NotFound("Template not found.");

            return Ok(updatedTemplate);
        }


        //להפוך את זה ללפי ID
        //[HttpGet("first")]
        //public async Task<IActionResult> GetFirstFile()
        //{
        //    var fileKey = await _templateService.GetFirstFileAsync();
        //    if (fileKey == null)
        //        return NotFound("לא נמצאו קבצים ב-S3");

        //    return Ok(fileKey);
        //}

    }
}
