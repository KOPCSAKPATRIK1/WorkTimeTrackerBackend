using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkTimeTracker.Core.DTOs;
using WorkTimeTracker.Core.Interfaces.Business;

namespace WorkTimeTracker.API.Controllers
{
    [Route("api/[controller]")]
    public class WorkTaskController : ControllerBase
    {
        private readonly IWorkTaskService _workTaskService;

        public WorkTaskController(IWorkTaskService workTaskService)
        {
            _workTaskService = workTaskService;
        }

        [HttpGet]
        public async Task<ActionResult<List<WorkTaskDto>>> GetAllTasks()
        {
            try
            {
                var tasks = await _workTaskService.GetAllWorkTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving tasks.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkTaskDto>> GetTask(int id)
        {
            try
            {
                var task = await _workTaskService.GetWorkTaskAsync(id);
                return Ok(task);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the task.", details = ex.Message });
            }
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<List<WorkTaskDto>>> GetTasksByProject(int projectId)
        {
            try
            {
                var tasks = await _workTaskService.GetWorkTasksByProjectAsync(projectId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving tasks.", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<WorkTaskDto>> CreateTask([FromBody] CreateWorkTaskRequest request)
        {
            try
            {
                // Aktuális bejelentkezett user ID kinyerése
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("User not authenticated.");
                }

                int currentUserId = int.Parse(userIdClaim);

                var createdTask = await _workTaskService.CreateWorkTaskAsync(request, currentUserId);

                return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating the task.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WorkTaskDto>> UpdateTask(int id, [FromBody] UpdateWorkTaskRequest request)
        {
            try
            {
                var updatedTask = await _workTaskService.UpdateWorkTaskAsync(id, request);
                return Ok(updatedTask);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the task.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                await _workTaskService.DeleteWorkTaskAsync(id);
                return Ok(new { message = "Task successfully deleted." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the task.", details = ex.Message });
            }
        }
    }
}
