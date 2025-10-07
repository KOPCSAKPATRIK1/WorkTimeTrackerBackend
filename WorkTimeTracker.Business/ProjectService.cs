using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTimeTracker.Core.Interfaces.Business;
using WorkTimeTracker.Core.Interfaces.Repository;
using WorkTimeTracker.Core.Models.Domain;
using WorkTimeTracker.Data;

namespace WorkTimeTracker.Business
{
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> _projectRepository;

        public ProjectService(IRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (string.IsNullOrWhiteSpace(project.Name))
                throw new ArgumentException("Project name is required.", nameof(project));

            project.CreatedAt = DateTime.Now;
            _projectRepository.Add(project);

            return project;
        }
    }


}
