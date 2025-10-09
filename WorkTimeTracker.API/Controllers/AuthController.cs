using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkTimeTracker.Core.DTOs;
using WorkTimeTracker.Core.Interfaces.Business;
using WorkTimeTracker.Core.Models.Domain;
using WorkTimeTracker.Repository.Data;

namespace WorkTimeTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly WorkTimeTrackerContext _context;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(WorkTimeTrackerContext context, IJwtTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            // Ellenőrzés: létezik-e már az email
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest("Ez az email cím már regisztrálva van.");
            }

            // Új user létrehozása (jelszó simán tárolva)
            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = registerDto.Password, // Egyszerűen tároljuk
                FullName = registerDto.FullName,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Token generálás
            var token = _jwtTokenService.GenerateToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            // User keresése email és jelszó alapján
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.PasswordHash == loginDto.Password);

            if (user == null)
            {
                return Unauthorized("Hibás email vagy jelszó.");
            }

            // Token generálás
            var token = _jwtTokenService.GenerateToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            };

            return Ok(response);
        }
    }
}
