using AskHire_Backend.Data.Entities;
using AskHire_Backend.Data.Repositories;
using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Interfaces.Services;
using AskHire_Backend.Repositories;
using AskHire_Backend.Repositories.Implementations;
using AskHire_Backend.Repositories.Interfaces;
using AskHire_Backend.Services;
using AskHire_Backend.Services.Implementations;
using AskHire_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ✅ Add Controllers
builder.Services.AddControllers();
builder.Services.AddHttpClient();


// ✅ Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Register AppDbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseString"))
           //.EnableSensitiveDataLogging() // Enable sensitive data logging for better debugging
           .LogTo(Console.WriteLine) // Log SQL queries to console
);

// ✅ Register Repositories & Services
builder.Services.AddScoped<IVacancyRepository, VacancyRepository>();
builder.Services.AddScoped<IVacancyService, VacancyService>();

// Register Question repository and service
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionService, QuestionService>();

// Register JobRole repository and service
builder.Services.AddScoped<IJobRoleRepository, JobRoleRepository>();
builder.Services.AddScoped<IJobRoleService, JobRoleService>();

builder.Services.AddScoped<IInterviewRepository, InterviewRepository>();
builder.Services.AddScoped<IInterviewService, InterviewService>();

builder.Services.AddScoped<IPreScreenTestRepository, PreScreenTestRepository>();
builder.Services.AddScoped<IPreScreenTestService, PreScreenTestService>();





var app = builder.Build();

// ✅ Enable Swagger for API Documentation
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AskHire API v1");
    c.RoutePrefix = string.Empty;  // Swagger loads as the default page
});

// ✅ Use Middleware
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();