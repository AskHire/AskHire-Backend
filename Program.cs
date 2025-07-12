using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Data.Repositories;
using AskHire_Backend.Data.Repositories.AdminRepositories;
using AskHire_Backend.Data.Repositories.CandidateRepositories;
using AskHire_Backend.Data.Repositories.ManagerRepositories;
using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Interfaces.Repositories.AdminRepositories;
using AskHire_Backend.Interfaces.Repositories.CandidateRepositories;
using AskHire_Backend.Interfaces.Repositories.IManagerRepositories;
using AskHire_Backend.Interfaces.Repositories.ManagerRepositories;
using AskHire_Backend.Interfaces.Services;
using AskHire_Backend.Interfaces.Services.ICandidateServices;
using AskHire_Backend.Interfaces.Services.IManagerServices;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Repositories;
using AskHire_Backend.Repositories.Interfaces;
using AskHire_Backend.Repositories.ManagerRepositories;
using AskHire_Backend.Services;
using AskHire_Backend.Services.AdminServices;
using AskHire_Backend.Services.CandidateServices;
using AskHire_Backend.Services.Interfaces;
using AskHire_Backend.Services.ManagerServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// CORS for frontend
// ==============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// ==============================
// Swagger
// ==============================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<CandidateFileService>();
builder.Services.AddHttpClient();

// ==============================
// Database Context
// ==============================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseString"))
           .LogTo(Console.WriteLine));

// ==============================
// Identity
// ==============================
builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ==============================
// JWT Authentication
// ==============================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["AppSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["AppSettings:Audience"],
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
        ValidateIssuerSigningKey = true
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["jwt"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ==============================
// Dependency Injection
// ==============================

// Common
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// Admin
builder.Services.AddScoped<IAdminJobRoleRepository, AdminJobRoleRepository>();
builder.Services.AddScoped<IAdminJobRoleService, AdminJobRoleService>();

builder.Services.AddScoped<IAdminNotificationRepository, AdminNotificationRepository>();
builder.Services.AddScoped<IAdminNotificationService, AdminNotificationService>();

builder.Services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

// Manager
builder.Services.AddScoped<IManagerInterviewRepository, ManagerInterviewRepository>();
builder.Services.AddScoped<IManagerInterviewService, ManagerInterviewService>();



builder.Services.AddScoped<IManagerEmailService, ManagerEmailService>();
builder.Services.AddScoped<IManagerNotificationRepository, ManagerNotificationRepository>();
builder.Services.AddScoped<IManagerNotificationService, ManagerNotificationService>();
builder.Services.AddScoped<IManagerDashboardRepository, ManagerDashboardRepository>();
builder.Services.AddScoped<IManagerDashboardService, ManagerDashboardService>();
builder.Services.AddScoped<IManagerCandidateRepository, ManagerCandidateRepository>();
builder.Services.AddScoped<IManagerCandidateService, ManagerCandidateService>();
builder.Services.AddScoped<IManagerLongListInterviewRepository, ManagerLongListInterviewRepository>();
builder.Services.AddScoped<IManagerLongListInterviewService, ManagerLongListInterviewService>();
builder.Services.AddScoped<IManagerLonglistIVacancyRepository, ManagerLonglistVacancyRepository>();

// Candidate
builder.Services.AddScoped<ICandidateVacancyRepository, CandidateVacancyRepository>();
builder.Services.AddScoped<ICandidateVacancyService, CandidateVacancyService>();

builder.Services.AddScoped<ICandidateDashboardRepository, CandidateDashboardRepository>();
builder.Services.AddScoped<ICandidateDashboardService, CandidateDashboardService>();

builder.Services.AddScoped<ICandidateInterviewRepository, CandidateInterviewRepository>();
builder.Services.AddScoped<ICandidateInterviewService, CandidateInterviewService>();

builder.Services.AddScoped<ICandidatePreScreenTestRepository, CandidatePreScreenTestRepository>();
builder.Services.AddScoped<ICandidatePreScreenTestService, CandidatePreScreenTestService>();

builder.Services.AddScoped<ICandidateAnswerCheckRepository, CandidateAnswerCheckRepository>();
builder.Services.AddScoped<ICandidateAnswerCheckService, CandidateAnswerCheckService>();

builder.Services.AddScoped<ICandidateFileRepository, CandidateFileRepository>();
builder.Services.AddScoped<ICandidateFileService, CandidateFileService>();

// Vacancy & Questions
builder.Services.AddScoped<IVacancyRepository, VacancyRepository>();
builder.Services.AddScoped<IVacancyService, VacancyService>();

builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionService, QuestionService>();

builder.Services.AddScoped<IJobRoleRepository, JobRoleRepository>();
builder.Services.AddScoped<IJobRoleService, JobRoleService>();

// ✅ Reminder Service Integration
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IReminderService, ReminderService>();

builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();

builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();

builder.Services.AddScoped<IManagerInterviewService, ManagerInterviewService>();





// ==============================
// App Pipeline
// ==============================
var app = builder.Build();

// Enable CORS
app.UseCors("AllowFrontend");

app.UseStaticFiles();

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var roles = new[] { "Admin", "Manager", "Candidate" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }
}

// Development tools
if (app.Environment.IsDevelopment())
{    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
