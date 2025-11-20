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
        Task<UserDto> GetUserAsync(int id);
        Task<List<UserDto>> GetAllUsersAsync();

    }
}
