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
    public class WorkTaskService : IWorkTaskService
    {
        private readonly IRepository<WorkTask> _taskRepository;
        private readonly IRepository<Project> _projectRepository;

        public WorkTaskService(IRepository<WorkTask> taskRepository, IRepository<Project> projectRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        public async Task<WorkTaskDto> CreateWorkTaskAsync(CreateWorkTaskRequest request, int createdByUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Task name is required.", nameof(request));

            // Ellenőrizzük, hogy létezik-e a projekt
            var projectExists = await _projectRepository.GetAll()
                .AnyAsync(p => p.Id == request.ProjectId);

            if (!projectExists)
                throw new KeyNotFoundException($"Project with ID {request.ProjectId} not found.");

            var task = new WorkTask
            {
                ProjectId = request.ProjectId,
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                AssignedToUserId = createdByUserId, // A létrehozó alapból a felelős
                IsDeleted = false
            };

            _taskRepository.Add(task);

            // Reload a taskot a kapcsolódó adatokkal
            var createdTask = await _taskRepository
                .GetAllIncluding(t => t.Project, t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == task.Id);

            return MapToDto(createdTask);
        }

        public async Task<WorkTaskDto> GetWorkTaskAsync(int id)
        {
            var task = await _taskRepository
                .GetAllIncluding(t => t.Project, t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found.");

            return MapToDto(task);
        }

        public async Task<List<WorkTaskDto>> GetAllWorkTasksAsync()
        {
            var tasks = await _taskRepository
                .GetAllIncluding(t => t.Project, t => t.AssignedToUser)
                .Where(t => !t.IsDeleted)
                .ToListAsync();

            return tasks.Select(t => MapToDto(t)).ToList();
        }

        public async Task<List<WorkTaskDto>> GetWorkTasksByProjectAsync(int projectId)
        {
            var tasks = await _taskRepository
                .GetAllIncluding(t => t.Project, t => t.AssignedToUser)
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .ToListAsync();

            return tasks.Select(t => MapToDto(t)).ToList();
        }

        public async Task<WorkTaskDto> UpdateWorkTaskAsync(int id, UpdateWorkTaskRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Task name is required.", nameof(request));

            var task = await _taskRepository
                .GetAllIncluding(t => t.Project, t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found.");

            task.Name = request.Name;
            task.Description = request.Description;
            task.AssignedToUserId = request.AssignedToUserId;

            _taskRepository.Update(task);

            // Reload a frissített taskot
            var updatedTask = await _taskRepository
                .GetAllIncluding(t => t.Project, t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            return MapToDto(updatedTask);
        }

        public async Task DeleteWorkTaskAsync(int id)
        {
            var task = await _taskRepository.GetAll()
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found.");

            // Soft delete
            task.IsDeleted = true;
            _taskRepository.Update(task);
        }

        private WorkTaskDto MapToDto(WorkTask task)
        {
            return new WorkTaskDto
            {
                Id = task.Id,
                ProjectId = task.ProjectId,
                ProjectName = task.Project?.Name,
                Name = task.Name,
                Description = task.Description,
                CreatedAt = task.CreatedAt,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUser?.FullName,
                IsDeleted = task.IsDeleted
            };
        }
    }
}
