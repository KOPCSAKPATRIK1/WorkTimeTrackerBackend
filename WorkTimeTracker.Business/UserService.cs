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
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<WorkTask> _taskRepository;

        public UserService(
            IRepository<User> userRepository,
            IRepository<Project> projectRepository,
            IRepository<WorkTask> taskRepository)
        {
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAll().ToListAsync();
            return users.Select(u => MapToDto(u)).ToList();
        }

        public async Task<ProjectDto> AssignUserToProjectAsync(int projectId, int userId)
        {
            // Ellenőrizzük, hogy létezik-e a user
            var user = await _userRepository.GetAll()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            // Ellenőrizzük, hogy létezik-e a projekt
            var project = await _projectRepository
                .GetAllIncluding(p => p.CreatedByUser, p => p.AssignedToUser)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                throw new KeyNotFoundException($"Project with ID {projectId} not found.");

            // User hozzárendelése
            project.AssignedToUserId = userId;
            _projectRepository.Update(project);

            // Frissített projekt lekérése
            var updatedProject = await _projectRepository
                .GetAllIncluding(p => p.CreatedByUser, p => p.AssignedToUser)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            return MapProjectToDto(updatedProject);
        }

        public async Task<ProjectDto> UnassignUserFromProjectAsync(int projectId)
        {
            var project = await _projectRepository
                .GetAllIncluding(p => p.CreatedByUser, p => p.AssignedToUser)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                throw new KeyNotFoundException($"Project with ID {projectId} not found.");

            project.AssignedToUserId = null;
            _projectRepository.Update(project);

            var updatedProject = await _projectRepository
                .GetAllIncluding(p => p.CreatedByUser, p => p.AssignedToUser)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            return MapProjectToDto(updatedProject);
        }

        public async Task AssignUserToTaskAsync(int taskId, int userId)
        {
            // Ellenőrizzük, hogy létezik-e a user
            var user = await _userRepository.GetAll()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            // Ellenőrizzük, hogy létezik-e a task
            var task = await _taskRepository.GetAll()
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {taskId} not found.");

            // User hozzárendelése
            task.AssignedToUserId = userId;
            _taskRepository.Update(task);
        }

        public async Task UnassignUserFromTaskAsync(int taskId)
        {
            var task = await _taskRepository.GetAll()
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {taskId} not found.");

            task.AssignedToUserId = null;
            _taskRepository.Update(task);
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                CreatedAt = user.CreatedAt
            };
        }

        private ProjectDto MapProjectToDto(Project project)
        {
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                ParentProjectId = project.ParentProjectId,
                CreatedAt = project.CreatedAt,
                CreatedByUserId = project.CreatedByUserId,
                CreatedByUserName = project.CreatedByUser?.FullName,
                AssignedToUserId = project.AssignedToUserId,
                AssignedToUserName = project.AssignedToUser?.FullName
            };
        }
    }
}
