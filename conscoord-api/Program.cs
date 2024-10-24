using System.Text.Json.Serialization;
using conscoord_api;
using conscoord_api.Controllers;
using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using conscoord_api.Services;
using Coravel;
using dotenv.net;
using Microsoft.EntityFrameworkCore;

var envVars = DotEnv.Read();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Cron Scheduler
builder.Services.AddScheduler();
builder.Services.AddScoped<SendEmailsAtMidnight>();
builder.Services.AddScoped<ShiftClockInReminder>();
//this is how you pass in parameters
//builder.Services.AddTransient<string>(p => "");

// Environment Variables
builder.Services.Configure<CustomConfiguration>(o =>
    {
        o.SMTP_SENDERNAME = Environment.GetEnvironmentVariable("SMTP_SENDERNAME") ?? envVars["SMTP_SENDERNAME"];
        o.SMTP_USERNAME = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? envVars["SMTP_USERNAME"];
        o.SMTP_PASSWORD = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? envVars["SMTP_PASSWORD"];
        o.DB = Environment.GetEnvironmentVariable("DB") ?? envVars["DB"];
        o.EMAIL_ENABLED = (Environment.GetEnvironmentVariable("EMAIL_ENABLED") ?? envVars["EMAIL_ENABLED"]) == "TRUE";
    }
);

// Services
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); // Prevent circular dependencies
builder.Services.AddDbContext<PostgresContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("DB") ?? envVars["DB"]));
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IProjectShiftService, ProjectShiftService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IEmployeeShiftService, EmployeeShiftService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IEmailService, EmailController>();
builder.Services.AddScoped<ShiftClockInReminder>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Health Probe
app.MapGet("/api/health", () => "healthy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Services.UseScheduler(scheduler =>
{
    //add more of these for different times/different processes
    scheduler.Schedule<SendEmailsAtMidnight>()
        .Cron("0 0 * * *")
        .PreventOverlapping(nameof(SendEmailsAtMidnight));

    scheduler.Schedule<ShiftClockInReminder>()
        .EveryFifteenMinutes()
        .PreventOverlapping(nameof(ShiftClockInReminder));
});

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
