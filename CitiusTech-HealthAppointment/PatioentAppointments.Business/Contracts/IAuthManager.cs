using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Dto;
using PatientAppointments.Business.Dtos;
using PatientAppointments.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Contracts
{
    public interface IAuthManager
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken);
        Task<ApplicationUser> GetUsersByName( string userName);
        Task<UserInfoDto> GetLoggedInUserInfo();
        Task<string> FetchOrCreateThreadForUser(string? staffId = null);
        Task<PersistentAgentThread> CreateThreadAsync();
        Task DeleteThreadForUserAsync();
    }
}
