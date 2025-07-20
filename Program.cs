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
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc; // Add this using directive

var builder = WebApplication.CreateBuilder(args);

// ==============================
// CORS for frontend
// ==============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:5173") // Changed to 3000 as per common React dev server
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
// Add Logging Services
// ==============================
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

// ==============================
// Database Context
// ==============================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseString"))
           .LogTo(Console.WriteLine));

// ==============================
// Identity Configuration
// ==============================
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    // Password settings - adjust as per your security requirements
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8; // Consistent with frontend regex
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    // VERY IMPORTANT: Set RequireConfirmedAccount to true to enforce email verification
    options.SignIn.RequireConfirmedAccount = true; // Changed to true
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders(); // Essential for GenerateEmailConfirmationTokenAsync

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
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero // No leeway
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["jwt"];
            return Task.CompletedTask;
        },
        // Optional: Customize 401 Unauthorized response for JWT challenges
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            var problemDetails = new ProblemDetails
            {
                Status = (int)System.Net.HttpStatusCode.Unauthorized,
                Title = "Authentication Required",
                Detail = "You are not authorized to access this resource or your session has expired. " +
                         (string.IsNullOrEmpty(context.ErrorDescription) ? "" : context.ErrorDescription),
                Type = "https://tools.ietf.org/html/rfc7807#section-3.1"
            };
            return context.Response.WriteAsJsonAsync(problemDetails);
        },
        // Optional: Customize 403 Forbidden response for JWT challenges (authorization failure)
        OnForbidden = context =>
        {
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";
            var problemDetails = new ProblemDetails
            {
                Status = (int)System.Net.HttpStatusCode.Forbidden,
                Title = "Access Denied",
                Detail = "You do not have permission to access this resource.",
                Type = "https://tools.ietf.org/html/rfc7807#section-3.1"
            };
            return context.Response.WriteAsJsonAsync(problemDetails);
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
builder.Services.AddTransient<IAuthEmailService, AuthEmailService>(); // Register your new email service here

// Admin (Existing services)
builder.Services.AddScoped<IAdminJobRoleRepository, AdminJobRoleRepository>();
builder.Services.AddScoped<IAdminJobRoleService, AdminJobRoleService>();
builder.Services.AddScoped<IAdminNotificationRepository, AdminNotificationRepository>();
builder.Services.AddScoped<IAdminNotificationService, AdminNotificationService>();
builder.Services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

// Manager (Existing services)
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

// Candidate (Existing services)
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

// Vacancy & Questions (Existing services)
builder.Services.AddScoped<IVacancyRepository, VacancyRepository>();
builder.Services.AddScoped<IVacancyService, VacancyService>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IJobRoleRepository, JobRoleRepository>();
builder.Services.AddScoped<IJobRoleService, JobRoleService>();

// Reminder Service Integration (Existing services)
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IManagerInterviewService, ManagerInterviewService>();
builder.Services.AddScoped<INotificationShowRepository, NotificationShowRepository>();
builder.Services.AddScoped<INotificationShowServices, NotificationShowService>();

// ==============================
// App Pipeline
// ==============================
var app = builder.Build();

// Enable CORS
app.UseCors("AllowFrontend");

app.UseStaticFiles();

// Seed roles on application startup (important for Identity)
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
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware order is crucial
app.UseRouting(); // Determines where the request should go
app.UseAuthentication(); // Verifies who the user is
app.UseAuthorization();   // Determines what the user can do
app.MapControllers();     // Maps routes to controller actions

app.Run();