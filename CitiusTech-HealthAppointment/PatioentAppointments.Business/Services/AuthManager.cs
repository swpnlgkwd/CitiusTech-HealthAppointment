using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Business.Dtos;
using PatientAppointments.Core.Contracts;
using PatientAppointments.Core.Entities;
using PatientAppointments.Infrastructure.Identity;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PatientAppointments.Business.Services
{
    public class AuthManager : IAuthManager
    {

        private readonly PersistentAgent _agent;
        private readonly PersistentAgentsClient _client;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _uow;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IGreetingManager _greetingService;
        private readonly IAgentConversationManager _agentConversationManager;
        private readonly ILogger<AuthManager> _logger;

        public AuthManager(
            UserManager<ApplicationUser> userManager, 
            IConfiguration config, 
            IUnitOfWork uow, 
            IHttpContextAccessor httpContextAccessor, 
            IGreetingManager greetingService, 
            IAgentConversationManager agentConversationManager, 
            ILogger<AuthManager> logger,
            PersistentAgent agent,
            PersistentAgentsClient client)
        {
            _userManager = userManager;
            _config = config;
            _uow = uow;
            _httpContextAccessor = httpContextAccessor;
            _greetingService = greetingService;
            _agentConversationManager = agentConversationManager;
            _logger = logger;
            _agent = agent;
            _client = client;
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

        public async Task<UserInfoDto> GetLoggedInUserInfo() {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                throw new Exception("User cannot be null");
            else
            {
                var role = user?.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";
                string userId = "";
                var fullName = "";
                if (role == "Provider")
                {
                    fullName = await _greetingService.GetProviderName(user);
                    userId = await _greetingService.GetProviderId(user);
                }
                else
                {
                    fullName = await _greetingService.GetPatientName(user);
                    userId = await _greetingService.GetPatientId(user);
                }
                return new UserInfoDto
                {
                    userFullName = fullName,
                    userRole = role,
                    userId = userId.ToString()
                };
            }           
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

            var thread = await FetchOrCreateThreadForUser(user.Id);

            return new AuthResponseDto(jwt, "dummy-refresh-token", expires, role , user.UserName ?? "", thread );
        }

        /// <summary>
        /// Fetches an existing thread for the user or creates a new one if none exists.
        /// </summary>
        public async Task<string> FetchOrCreateThreadForUser(string? userId)
        {
            try
            {
                // Try to resolve staffId from context if not passed
                if (string.IsNullOrEmpty(userId))
                {
                    var contextStaffId = (await GetLoggedInUserInfo()).userId;
                    if (!string.IsNullOrEmpty(contextStaffId))
                        userId = contextStaffId.ToString();
                }

                // If staffId is known, check for existing thread
                if (!string.IsNullOrEmpty(userId))
                {
                    var existingThreadId = await _agentConversationManager.FetchThreadIdForLoggedInUser(userId);
                    if (!string.IsNullOrEmpty(existingThreadId))
                    {
                        _logger.LogInformation("Existing thread found for StaffId {StaffId}: {ThreadId}", userId, existingThreadId);
                        return existingThreadId;
                    }
                }

                // No thread found — create a new one
                var newThread = await CreateThreadAsync();

                //If we have staffId, store the conversation
                if (!string.IsNullOrEmpty(userId))
                {
                    var agentConversation = new AgentConversations
                    {
                        user_id = userId,
                        thread_id = newThread.Id,
                        created_at = DateTime.UtcNow
                    };

                    await _agentConversationManager.AddAgentConversation(agentConversation);
                    _logger.LogInformation("New thread {ThreadId} created and saved for StaffId {StaffId}", newThread.Id, userId);
                }
                else
                {
                    _logger.LogInformation("New thread {ThreadId} created for unauthenticated user (no staffId yet)", newThread.Id);
                }

                return newThread.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch or create thread for user.");
                throw;
            }
        }

        /// <summary>
        /// Creates a new persistent agent thread for communication.
        /// </summary>
        public async Task<PersistentAgentThread> CreateThreadAsync()
        {
            try
            {
                _logger.LogInformation("Creating new persistent agent thread...");
                var thread = await _client.Threads.CreateThreadAsync();
                _logger.LogInformation("Successfully created thread with ID: {ThreadId}", thread.Value.Id);
                return thread;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating new persistent agent thread.");
                throw;
            }
        }

        /// <summary>
        /// Deletes a thread from OpenAI and your system.
        /// </summary>
        public async Task DeleteThreadForUserAsync()
        {
            var userId = (await GetLoggedInUserInfo()).userId;
            var threadId = await _agentConversationManager.FetchThreadIdForLoggedInUser(userId);

            if (string.IsNullOrWhiteSpace(threadId))
            {
                _logger.LogWarning("ThreadId is null or empty. Skipping thread deletion.");
                return;
            }

            try
            {
                _logger.LogInformation("Deleting thread with ID: {ThreadId}", threadId);
                await _client.Threads.DeleteThreadAsync(threadId);
                _logger.LogInformation("Successfully deleted thread from OpenAI: {ThreadId}", threadId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while deleting thread from OpenAI: {ThreadId}", threadId);
            }

            try
            {
                var agentConversation = await _agentConversationManager.FetchLoggedInUserAgentConversationInfo();
                if (agentConversation != null)
                {
                    await _agentConversationManager.DeleteAgentConversation(agentConversation);
                    _logger.LogInformation("Deleted agent conversation entry for ThreadId: {ThreadId}", threadId);
                }
                else
                {
                    _logger.LogInformation("No agent conversation found to delete for ThreadId: {ThreadId}", threadId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting agent conversation for thread {ThreadId}", threadId);
                throw;
            }
        }

    }
}
