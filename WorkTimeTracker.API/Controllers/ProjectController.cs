using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkTimeTracker.Business;
using WorkTimeTracker.Core.DTOs;
using WorkTimeTracker.Core.Interfaces.Business;
using WorkTimeTracker.Core.Models.Domain;

namespace WorkTimeTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectDto>>> GetAllProjects()
        {
            try
            {
                var projects = await _projectService.GetAllProjectsAsync();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving projects.", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectRequest request)
        {
            try
            {
                // Aktuális bejelentkezett user ID kinyerése a JWT tokenből
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("User not authenticated.");
                }

                int currentUserId = int.Parse(userIdClaim);

                var project = new Project
                {
                    Name = request.Name,
                    Description = request.Description,
                    ParentProjectId = request.ParentProjectId,
                    CreatedByUserId = currentUserId, // Bejelentkezett user
                    AssignedToUserId = request.AssignedToUserId
                };

                var createdProject = await _projectService.CreateProjectAsync(project);

                return CreatedAtAction(nameof(GetProject), new { id = createdProject.Id }, createdProject);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating the project.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {

        }

    }
}
