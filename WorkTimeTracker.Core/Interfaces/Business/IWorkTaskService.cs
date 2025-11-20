using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTimeTracker.Core.DTOs;

namespace WorkTimeTracker.Core.Interfaces.Business
{
    public interface IWorkTaskService
    {
        Task<WorkTaskDto> CreateWorkTaskAsync(CreateWorkTaskRequest request, int createdByUserId);
        Task<WorkTaskDto> GetWorkTaskAsync(int id);
        Task<List<WorkTaskDto>> GetAllWorkTasksAsync();
        Task<List<WorkTaskDto>> GetWorkTasksByProjectAsync(int projectId);
        Task<WorkTaskDto> UpdateWorkTaskAsync(int id, UpdateWorkTaskRequest request);
        Task DeleteWorkTaskAsync(int id);
    }
}
