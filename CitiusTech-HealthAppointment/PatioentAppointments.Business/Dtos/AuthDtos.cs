using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Dtos
{
    public record RegisterDto(string Email, string Password, string Role, string UserName);

    public record LoginDto(string Email, string Password);

    public record AuthResponseDto(string Token, string RefreshToken, DateTime ExpiresAt, string role, string UserName);
}
