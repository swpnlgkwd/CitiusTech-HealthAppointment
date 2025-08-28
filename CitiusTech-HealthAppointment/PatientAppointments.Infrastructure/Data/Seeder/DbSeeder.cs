using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientAppointments.Core.Entities;
using PatientAppointments.Infrastructure.Data;
using PatientAppointments.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientAppointments.Infrastructure
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await context.Database.MigrateAsync();

            // Seed Users
            if (!userManager.Users.Any())
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");

                var doctorUser = new ApplicationUser
                {
                    UserName = "Provider",
                    Email = "Provider@example.com",
                    EmailConfirmed = true,
                    DoctorId = 1
                };
                await userManager.CreateAsync(doctorUser, "Provider@123");
                await userManager.AddToRoleAsync(doctorUser, "Provider");

                var patientUser = new ApplicationUser
                {
                    UserName = "patient1",
                    Email = "patient1@example.com",
                    EmailConfirmed = true,
                    PatientId = 1
                };
                await userManager.CreateAsync(patientUser, "Patient@123");
                await userManager.AddToRoleAsync(patientUser, "Patient");
            }
        }
    }
}
