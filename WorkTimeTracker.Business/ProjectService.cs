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

        public ProjectService(IRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ProjectDto> CreateProjectAsync(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (string.IsNullOrWhiteSpace(project.Name))
                throw new ArgumentException("Project name is required.", nameof(project));

            project.CreatedAt = DateTime.UtcNow;
            _projectRepository.Add(project);

            // Reload a projektet a kapcsolódó adatokkal
            var createdProject = await _projectRepository
                .GetAllIncluding(p => p.CreatedByUser, p => p.AssignedToUser)
                .FirstOrDefaultAsync(p => p.Id == project.Id);

            return MapToDto(createdProject);
        }

        public Task DeleteProject(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectDto> EditProject(Project project)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository
                .GetAllIncluding(p => p.CreatedByUser, p => p.AssignedToUser)
                .ToListAsync();

            return projects.Select(p => MapToDto(p)).ToList();
        }

        public ProjectDto GetProject(int id)
        {
            var project =  _projectRepository.Get(id);

            return MapToDto(project);
        }

        private ProjectDto MapToDto(Project project)
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
