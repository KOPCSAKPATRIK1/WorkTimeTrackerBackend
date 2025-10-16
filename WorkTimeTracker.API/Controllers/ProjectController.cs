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
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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

                return  createdProject;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating the project.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task< ActionResult<ProjectDto>> GetProject(int id)
        {
            return await _projectService.GetProjectAsync(id);
        }

        [HttpPut]
        public async Task<ActionResult<ProjectDto>> Edit([FromBody] EditProjectRequest request)
        {
            try
            {
                var project = new Project
                {
                    Name = request.Name,
                    Description = request.Description,
                    ParentProjectId = request.Id
                };

                var editedProject = await _projectService.EditProject(request);

                return editedProject;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while editing the project.", details = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteProject(int id)
        {
            try
            {
                await _projectService.DeleteProjectAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while editing the project.", details = ex.Message });
            }
        }

    }
}
