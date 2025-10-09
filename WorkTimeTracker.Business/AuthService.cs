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
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(IRepository<User> userRepository, IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Ellenőrzés: létezik-e már az email
            var existingUser = await _userRepository
                .Find(u => u.Email == registerDto.Email)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                throw new InvalidOperationException("Ez az email cím már regisztrálva van.");
            }

            // Új user létrehozása
            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = registerDto.Password,
                FullName = registerDto.FullName,
                CreatedAt = DateTime.UtcNow
            };

            _userRepository.Add(user);

            return new AuthResponseDto
            {
                Token = _jwtTokenService.GenerateToken(user),
                Email = user.Email,
                FullName = user.FullName,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // User keresése email és jelszó alapján
            var user = await _userRepository
                .Find(u => u.Email == loginDto.Email && u.PasswordHash == loginDto.Password)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new UnauthorizedAccessException("Hibás email vagy jelszó.");
            }

            return new AuthResponseDto
            {
                Token = _jwtTokenService.GenerateToken(user),
                Email = user.Email,
                FullName = user.FullName,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            };
        }

        public void Logout()
        {
            // JWT-vel a kijelentkezés client oldalon történik
            // A token törlése a client oldalon történik (localStorage/sessionStorage)
            // Itt opcionálisan lehetne token blacklist-et kezelni
            // Jelenleg csak egy üres metódus, ami jelzi hogy a logout megtörtént
        }
    }
}
