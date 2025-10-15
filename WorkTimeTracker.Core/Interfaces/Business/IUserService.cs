using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTimeTracker.Core.DTOs;

namespace WorkTimeTracker.Core.Interfaces.Business
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<ProjectDto> AssignUserToProjectAsync(int projectId, int userId);
        Task<ProjectDto> UnassignUserFromProjectAsync(int projectId);
        Task AssignUserToTaskAsync(int taskId, int userId);
        Task UnassignUserFromTaskAsync(int taskId);
    }
}
