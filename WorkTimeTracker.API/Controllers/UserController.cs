using Microsoft.AspNetCore.Mvc;
using WorkTimeTracker.Core.DTOs;
using WorkTimeTracker.Core.Interfaces.Business;

namespace WorkTimeTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("assign-to-project/{projectId}")]
        public async Task<ActionResult<ProjectDto>> AssignUserToProject(int projectId, [FromBody] AssignUserRequest request)
        {
            try
            {
                var project = await _userService.AssignUserToProjectAsync(projectId, request.UserId);
                return Ok(project);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while assigning user to project.", details = ex.Message });
            }
        }

        [HttpDelete("unassign-from-project/{projectId}")]
        public async Task<ActionResult<ProjectDto>> UnassignUserFromProject(int projectId)
        {
            try
            {
                var project = await _userService.UnassignUserFromProjectAsync(projectId);
                return Ok(project);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while unassigning user from project.", details = ex.Message });
            }
        }

        [HttpPost("assign-to-task/{taskId}")]
        public async Task<IActionResult> AssignUserToTask(int taskId, [FromBody] AssignUserRequest request)
        {
            try
            {
                await _userService.AssignUserToTaskAsync(taskId, request.UserId);
                return Ok(new { message = "User successfully assigned to task." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while assigning user to task.", details = ex.Message });
            }
        }

        [HttpDelete("unassign-from-task/{taskId}")]
        public async Task<IActionResult> UnassignUserFromTask(int taskId)
        {
            try
            {
                await _userService.UnassignUserFromTaskAsync(taskId);
                return Ok(new { message = "User successfully unassigned from task." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while unassigning user from task.", details = ex.Message });
            }
        }
    }
}
