using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTimeTracker.Core.DTOs;

namespace WorkTimeTracker.Core.Interfaces.Business
{
    public interface ITimeEntryService
    {
        Task<TimeEntryDto> CreateTimeEntryAsync(CreateTimeEntryRequest request, int userId);
        Task<TimeEntryDto> GetTimeEntryAsync(int id);
        Task<List<TimeEntryDto>> GetAllTimeEntriesAsync();
        Task<List<TimeEntryDto>> GetTimeEntriesByUserAsync(int userId);
        Task<List<TimeEntryDto>> GetTimeEntriesByProjectAsync(int projectId);
        Task<List<TimeEntryDto>> GetTimeEntriesByTaskAsync(int taskId);
        Task<TimeEntryDto> UpdateTimeEntryAsync(int id, UpdateTimeEntryRequest request);
        Task DeleteTimeEntryAsync(int id);
    }
}
