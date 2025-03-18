using CVBuilder.Core.Models;
using CVBuilder.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CVBuilder.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplateController(ITemplateService templateService, IUserService userService)
        {
            _templateService = templateService;
        }
        private int GetUserIdFromContext()
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                return int.Parse(userIdClaim.Value);
            }
            throw new UnauthorizedAccessException("User not authenticated.");
        }

        // קבלת כל התבניות למשתמש מחובר
        [HttpGet]
        [Authorize]
        public IActionResult GetTemplates()
        {
            var templates = _templateService.GetAllTemplates();  
            return Ok(templates);
        }
        // קבלת תבנית לפי id למשתמש מחובר
        [HttpGet("{id}")]
        public IActionResult GetTemplateById(int id)
        {
            var userId = GetUserIdFromContext();

            if (userId == null)
            {
                return Unauthorized("User is not authenticated.");  // אם אין משתמש מחובר
            }

            var templates = _templateService.GetAllTemplates();
            return Ok(templates);
        }

        // הוספת תבנית – רק למנהל
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddTemplate([FromBody] Template template)
        {
            if (template == null)
                return BadRequest();

            _templateService.AddTemplate(template);
            return CreatedAtAction(nameof(GetTemplateById), new { id = template.Id }, template);
        }

        // מחיקת תבנית – רק למנהל
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteTemplate(int id)
        {
            var success = _templateService.DeleteTemplate(id);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
