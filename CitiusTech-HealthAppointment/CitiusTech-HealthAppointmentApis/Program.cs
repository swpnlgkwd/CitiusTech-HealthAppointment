using Azure.AI.Agents.Persistent;
using Azure.Identity;
using CitiusTech_HealthAppointmentApis.Agent;
using CitiusTech_HealthAppointmentApis.Agent.AgentStore;
using CitiusTech_HealthAppointmentApis.Agent.Handler;
using CitiusTech_HealthAppointmentApis.Agent.Handler.Appointment;
using CitiusTech_HealthAppointmentApis.Agent.Handler.HelperToolsHander;
using CitiusTech_HealthAppointmentApis.Agent.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Business.Contracts.Risk;
using PatientAppointments.Business.Services;
using PatientAppointments.Business.Services.Risk;
using PatientAppointments.Core.Contracts;
using PatientAppointments.Infrastructure;
using PatientAppointments.Infrastructure.Data;
using PatientAppointments.Infrastructure.Identity;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Read configuration
var configuration = builder.Configuration;


builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt => {
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<AppDbContext>()
  .AddDefaultTokenProviders();

var keyBytes = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options => {
    options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
    options.AddPolicy("RequireDoctor", p => p.RequireRole("Provider"));
    options.AddPolicy("RequirePatient", p => p.RequireRole("Patient"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin() // ?? allows any origin
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Agent infrastructure
builder.Services.AddSingleton<IAgentStore, FileAgentStore>(); 
builder.Services.AddSingleton<IAgentManager, AgentManager>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<GreetingManager>();
// Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPatientRiskManager, PatientRiskManager>();

// Business
builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(IAppointmentManager)) // any type from the assembly containing your managers
    .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Manager")))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);



// DI Auto
builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(AppDbContext)) // or any type in Infrastructure project
    .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);

// Register services
builder.Services.AddToolHandlers();  // This automatically registers all IToolHandler implementations


builder.Services.AddScoped<PersistentAgent>(sp =>
{
    var agentManager = sp.GetRequiredService<IAgentManager>();
    return agentManager.GetAgent();
});

builder.Services.AddScoped<IAgentService, AgentService>(sp =>
{
    var client = sp.GetRequiredService<PersistentAgentsClient>();
    var agentManager = sp.GetRequiredService<IAgentManager>();
    var agent = agentManager.GetAgent(); // returns a PersistentAgent (already built)      
    var logger = sp.GetRequiredService<ILogger<AgentService>>();
    var toolHandlers = sp.GetServices<IToolHandler>();
    var acm = sp.GetRequiredService<IAgentConversationManager>();
    var am = sp.GetRequiredService<IAuthManager>();

    return new AgentService(client, agent, toolHandlers, acm, logger, am);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin() // ?? allows any origin
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<PersistentAgentsClient>(sp =>
{
    var endpoint = configuration["ProjectEndpoint"];
    return new PersistentAgentsClient(endpoint, new DefaultAzureCredential());
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure the agent is created at startup
using (var scope = app.Services.CreateScope())
{
    var agentManager = scope.ServiceProvider.GetRequiredService<IAgentManager>();
    await agentManager.EnsureAgentExistsAsync();

    //Seeder for logins
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await DbSeeder.SeedAsync(context, userManager, roleManager);
}
app.Run();
