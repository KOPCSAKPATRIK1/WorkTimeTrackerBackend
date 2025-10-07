using Microsoft.AspNetCore.Mvc;
using WorkTimeTracker.Business;
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

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
        {
            try
            {
                var project = new Project
                {
                    Name = request.Name,
                    Description = request.Description,
                    CreatedByUserId = 1
                };

                var createdProject = await _projectService.CreateProjectAsync(project);

                return Ok();
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

        public class CreateProjectRequest
        {
            public string Name { get; set; }
            public string? Description { get; set; }
        }

    }
}
