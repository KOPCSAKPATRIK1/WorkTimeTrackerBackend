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

            // DTO-ra konvertálás
            return new ProjectDto
            {
                Id = createdProject.Id,
                Name = createdProject.Name,
                Description = createdProject.Description,
                ParentProjectId = createdProject.ParentProjectId,
                CreatedAt = createdProject.CreatedAt,
                CreatedByUserId = createdProject.CreatedByUserId,
                CreatedByUserName = createdProject.CreatedByUser?.FullName,
                AssignedToUserId = createdProject.AssignedToUserId,
                AssignedToUserName = createdProject.AssignedToUser?.FullName
            };
        }
    }



}
