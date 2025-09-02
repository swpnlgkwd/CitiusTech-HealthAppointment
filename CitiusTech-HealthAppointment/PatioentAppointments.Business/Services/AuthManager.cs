using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Business.Dtos;
using PatientAppointments.Core.Contracts;
using PatientAppointments.Infrastructure.Identity;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PatientAppointments.Business.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _uow;
        private IHttpContextAccessor _httpContextAccessor;

        public AuthManager(UserManager<ApplicationUser> userManager, IConfiguration config, IUnitOfWork uow, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _config = config;
            _uow = uow;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser { UserName = dto.UserName, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, dto.Role);

            return await GenerateToken(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                throw new UnauthorizedAccessException("Invalid credentials");
            return await GenerateToken(user);
        }

        public Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken)
        {
            // TODO: implement refresh token persistence
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> GetUsersByName(string name) => await _userManager.FindByNameAsync(name);

        public async Task<IList<string>> GetUserRole() {
            var user = _httpContextAccessor.HttpContext?.User;
            var usr = await _userManager.GetUserAsync(user);
            return await _userManager.GetRolesAsync(usr);
        } 

        private async Task<AuthResponseDto> GenerateToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Patient";
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, role),
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));


            foreach (var r in roles)
            {
                if (r == "Provider")
                {
                    var appUser = await _userManager.FindByIdAsync(user.Id);

                    if (appUser?.DoctorId != null && appUser.DoctorId > 0)
                    {
                        claims.Add(new Claim("ProviderId", appUser.DoctorId.Value.ToString()));
                    }
                }

                if (r == "Patient")
                {
                    var appUser = await _userManager.FindByIdAsync(user.Id);

                    if (appUser?.PatientId != null && appUser.PatientId > 0)
                    {
                        claims.Add(new Claim("PatientId", appUser.PatientId.Value.ToString()));
                    }
                }

                if (r == "Admin")
                {
                    claims.Add(new Claim("AdminLevel", "Super"));
                    // optional, can add extra info for admins
                }
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponseDto(jwt, "dummy-refresh-token", expires, role ,  UserName: user.UserName );
        }
    }
}
