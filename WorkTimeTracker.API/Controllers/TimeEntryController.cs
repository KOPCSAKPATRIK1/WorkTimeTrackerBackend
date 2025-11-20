using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkTimeTracker.Core.DTOs;
using WorkTimeTracker.Core.Interfaces.Business;

namespace WorkTimeTracker.API.Controllers
{
    [Route("api/[controller]")]
    public class TimeEntryController : Controller
    {
        private readonly ITimeEntryService _timeEntryService;

        public TimeEntryController(ITimeEntryService timeEntryService)
        {
            _timeEntryService = timeEntryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TimeEntryDto>>> GetAllTimeEntries()
        {
            try
            {
                var timeEntries = await _timeEntryService.GetAllTimeEntriesAsync();
                return Ok(timeEntries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving time entries.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TimeEntryDto>> GetTimeEntry(int id)
        {
            try
            {
                var timeEntry = await _timeEntryService.GetTimeEntryAsync(id);
                return Ok(timeEntry);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the time entry.", details = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<TimeEntryDto>>> GetTimeEntriesByUser(int userId)
        {
            try
            {
                var timeEntries = await _timeEntryService.GetTimeEntriesByUserAsync(userId);
                return Ok(timeEntries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving time entries.", details = ex.Message });
            }
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<List<TimeEntryDto>>> GetTimeEntriesByProject(int projectId)
        {
            try
            {
                var timeEntries = await _timeEntryService.GetTimeEntriesByProjectAsync(projectId);
                return Ok(timeEntries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving time entries.", details = ex.Message });
            }
        }

        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<List<TimeEntryDto>>> GetTimeEntriesByTask(int taskId)
        {
            try
            {
                var timeEntries = await _timeEntryService.GetTimeEntriesByTaskAsync(taskId);
                return Ok(timeEntries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving time entries.", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<TimeEntryDto>> CreateTimeEntry([FromBody] CreateTimeEntryRequest request)
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

                var createdTimeEntry = await _timeEntryService.CreateTimeEntryAsync(request, currentUserId);

                return CreatedAtAction(nameof(GetTimeEntry), new { id = createdTimeEntry.Id }, createdTimeEntry);
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
                return StatusCode(500, new { error = "An error occurred while creating the time entry.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TimeEntryDto>> UpdateTimeEntry(int id, [FromBody] UpdateTimeEntryRequest request)
        {
            try
            {
                var updatedTimeEntry = await _timeEntryService.UpdateTimeEntryAsync(id, request);
                return Ok(updatedTimeEntry);
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
                return StatusCode(500, new { error = "An error occurred while updating the time entry.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeEntry(int id)
        {
            try
            {
                await _timeEntryService.DeleteTimeEntryAsync(id);
                return Ok(new { message = "Time entry successfully deleted." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the time entry.", details = ex.Message });
            }
        }
    }
}
