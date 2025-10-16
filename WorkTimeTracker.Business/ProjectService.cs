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
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<User> _userRepository;

        public ProjectService(IRepository<Project> projectRepository, IRepository<User> userRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        public async Task<ProjectDto> CreateProjectAsync(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (string.IsNullOrWhiteSpace(project.Name))
                throw new ArgumentException("Project name is required.", nameof(project));

            project.CreatedAt = DateTime.UtcNow;


            project.CreatedByUser = _userRepository.Get(project.CreatedByUserId);
            _projectRepository.Add(project);

            // Reload a projektet a kapcsolódó adatokkal
            var createdProject = await _projectRepository
                .GetAllIncluding(p => p.CreatedByUser, p => p.AssignedToUser)
                .FirstOrDefaultAsync(p => p.Id == project.Id);

            return MapToDto(createdProject);
        }

        public async Task DeleteProjectAsync(int id)
        {
            var x = await _projectRepository.GetAsync(id);
            x.IsDeleted = true;
            _projectRepository.Update(x);
        }

        public async Task<ProjectDto> EditProject(EditProjectRequest project)
        {
            var x = await _projectRepository.GetAsync(project.Id);
            x.Description = project.Description;
            x.Name = project.Name;
            x.ParentProjectId = project.ParentProjectId;
            _projectRepository.Update(x);

            var editedProject = await _projectRepository
            .GetAllIncluding(p => p.CreatedByUser, p => p.AssignedToUser)
            .FirstOrDefaultAsync(p => p.Id == project.Id);

            return MapToDto(editedProject);

        }

        public async Task<List<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository
                .GetAllIncluding(p => p.CreatedByUser, p => p.AssignedToUser).Where(p => !p.IsDeleted)
                .ToListAsync();

            return projects.Select(p => MapToDto(p)).ToList();
        }

        public async Task<ProjectDto> GetProjectAsync(int id)
        {
            var project = await _projectRepository
                            .GetAllIncluding(p => p.CreatedByUser, p => p.AssignedToUser)
                            .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                throw new KeyNotFoundException($"Project with ID {id} not found.");

            return MapToDto(project);
        }

        private ProjectDto MapToDto(Project project)
        {
            var x = new ProjectDto
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
            return x;
        }
    }



}
