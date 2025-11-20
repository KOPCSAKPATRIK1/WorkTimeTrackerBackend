using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTimeTracker.Core.DTOs;
using WorkTimeTracker.Core.Interfaces.Business;
using WorkTimeTracker.Core.Interfaces.Repository;
using WorkTimeTracker.Core.Models.Domain;

namespace WorkTimeTracker.Business
{
    public class TimeEntryService : ITimeEntryService
    {
        private readonly IRepository<TimeEntry> _timeEntryRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<WorkTask> _taskRepository;

        public TimeEntryService(
            IRepository<TimeEntry> timeEntryRepository,
            IRepository<User> userRepository,
            IRepository<Project> projectRepository,
            IRepository<WorkTask> taskRepository)
        {
            _timeEntryRepository = timeEntryRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
        }

        public async Task<TimeEntryDto> CreateTimeEntryAsync(CreateTimeEntryRequest request, int userId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Hours <= 0)
                throw new ArgumentException("Hours must be greater than zero.", nameof(request.Hours));

            // Ellenőrizzük, hogy létezik-e a user
            var userExists = await _userRepository.GetAll()
                .AnyAsync(u => u.Id == userId);

            if (!userExists)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            // Ellenőrizzük, hogy létezik-e a projekt (ha meg van adva)
            if (request.ProjectId.HasValue)
            {
                var projectExists = await _projectRepository.GetAll()
                    .AnyAsync(p => p.Id == request.ProjectId.Value);

                if (!projectExists)
                    throw new KeyNotFoundException($"Project with ID {request.ProjectId.Value} not found.");
            }

            // Ellenőrizzük, hogy létezik-e a task (ha meg van adva)
            if (request.TaskId.HasValue)
            {
                var taskExists = await _taskRepository.GetAll()
                    .AnyAsync(t => t.Id == request.TaskId.Value && !t.IsDeleted);

                if (!taskExists)
                    throw new KeyNotFoundException($"Task with ID {request.TaskId.Value} not found.");
            }

            var timeEntry = new TimeEntry
            {
                UserId = userId,
                ProjectId = request.ProjectId,
                TaskId = request.TaskId,
                Date = request.Date,
                Hours = request.Hours,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            _timeEntryRepository.Add(timeEntry);

            // Reload a time entry-t a kapcsolódó adatokkal
            var createdEntry = await _timeEntryRepository
                .GetAllIncluding(te => te.User, te => te.Project, te => te.Task)
                .FirstOrDefaultAsync(te => te.Id == timeEntry.Id);

            return MapToDto(createdEntry);
        }

        public async Task<TimeEntryDto> GetTimeEntryAsync(int id)
        {
            var timeEntry = await _timeEntryRepository
                .GetAllIncluding(te => te.User, te => te.Project, te => te.Task)
                .FirstOrDefaultAsync(te => te.Id == id);

            if (timeEntry == null)
                throw new KeyNotFoundException($"Time entry with ID {id} not found.");

            return MapToDto(timeEntry);
        }

        public async Task<List<TimeEntryDto>> GetAllTimeEntriesAsync()
        {
            var timeEntries = await _timeEntryRepository
                .GetAllIncluding(te => te.User, te => te.Project, te => te.Task)
                .ToListAsync();

            return timeEntries.Select(te => MapToDto(te)).ToList();
        }

        public async Task<List<TimeEntryDto>> GetTimeEntriesByUserAsync(int userId)
        {
            var timeEntries = await _timeEntryRepository
                .GetAllIncluding(te => te.User, te => te.Project, te => te.Task)
                .Where(te => te.UserId == userId)
                .ToListAsync();

            return timeEntries.Select(te => MapToDto(te)).ToList();
        }

        public async Task<List<TimeEntryDto>> GetTimeEntriesByProjectAsync(int projectId)
        {
            var timeEntries = await _timeEntryRepository
                .GetAllIncluding(te => te.User, te => te.Project, te => te.Task)
                .Where(te => te.ProjectId == projectId)
                .ToListAsync();

            return timeEntries.Select(te => MapToDto(te)).ToList();
        }

        public async Task<List<TimeEntryDto>> GetTimeEntriesByTaskAsync(int taskId)
        {
            var timeEntries = await _timeEntryRepository
                .GetAllIncluding(te => te.User, te => te.Project, te => te.Task)
                .Where(te => te.TaskId == taskId)
                .ToListAsync();

            return timeEntries.Select(te => MapToDto(te)).ToList();
        }

        public async Task<TimeEntryDto> UpdateTimeEntryAsync(int id, UpdateTimeEntryRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Hours <= 0)
                throw new ArgumentException("Hours must be greater than zero.", nameof(request.Hours));

            var timeEntry = await _timeEntryRepository
                .GetAllIncluding(te => te.User, te => te.Project, te => te.Task)
                .FirstOrDefaultAsync(te => te.Id == id);

            if (timeEntry == null)
                throw new KeyNotFoundException($"Time entry with ID {id} not found.");

            // Ellenőrizzük, hogy létezik-e a projekt (ha meg van adva)
            if (request.ProjectId.HasValue)
            {
                var projectExists = await _projectRepository.GetAll()
                    .AnyAsync(p => p.Id == request.ProjectId.Value);

                if (!projectExists)
                    throw new KeyNotFoundException($"Project with ID {request.ProjectId.Value} not found.");
            }

            // Ellenőrizzük, hogy létezik-e a task (ha meg van adva)
            if (request.TaskId.HasValue)
            {
                var taskExists = await _taskRepository.GetAll()
                    .AnyAsync(t => t.Id == request.TaskId.Value && !t.IsDeleted);

                if (!taskExists)
                    throw new KeyNotFoundException($"Task with ID {request.TaskId.Value} not found.");
            }

            // Frissítés
            timeEntry.ProjectId = request.ProjectId;
            timeEntry.TaskId = request.TaskId;
            timeEntry.Date = request.Date;
            timeEntry.Hours = request.Hours;
            timeEntry.Description = request.Description;

            _timeEntryRepository.Update(timeEntry);

            // Reload a frissített time entry-t
            var updatedEntry = await _timeEntryRepository
                .GetAllIncluding(te => te.User, te => te.Project, te => te.Task)
                .FirstOrDefaultAsync(te => te.Id == id);

            return MapToDto(updatedEntry);
        }

        public async Task DeleteTimeEntryAsync(int id)
        {
            var timeEntry = await _timeEntryRepository.GetAll()
                .FirstOrDefaultAsync(te => te.Id == id);

            if (timeEntry == null)
                throw new KeyNotFoundException($"Time entry with ID {id} not found.");

            _timeEntryRepository.Remove(timeEntry);
        }

        private TimeEntryDto MapToDto(TimeEntry timeEntry)
        {
            return new TimeEntryDto
            {
                Id = timeEntry.Id,
                UserId = timeEntry.UserId,
                UserName = timeEntry.User?.FullName,
                ProjectId = timeEntry.ProjectId,
                ProjectName = timeEntry.Project?.Name,
                TaskId = timeEntry.TaskId,
                TaskName = timeEntry.Task?.Name,
                Date = timeEntry.Date,
                Hours = timeEntry.Hours,
                Description = timeEntry.Description,
                CreatedAt = timeEntry.CreatedAt
            };
        }
    }
}
