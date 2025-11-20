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

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> GetUserAsync(int id)
        {
            var user = await _userRepository.GetAll()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found.");

            return MapToDto(user);
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAll().ToListAsync();
            return users.Select(u => MapToDto(u)).ToList();
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
    }
}
