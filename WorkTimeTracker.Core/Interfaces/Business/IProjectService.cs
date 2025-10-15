using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTimeTracker.Core.DTOs;
using WorkTimeTracker.Core.Models.Domain;

namespace WorkTimeTracker.Core.Interfaces.Business
{
    public interface IProjectService
    {
        Task<ProjectDto> CreateProjectAsync(Project project);
        Task<List<ProjectDto>> GetAllProjectsAsync();
        Task<ProjectDto> EditProject(Project project);

        Task<ProjectDto> GetProject(int id);
        Task DeleteProject(int id);
    }
}
