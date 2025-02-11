using System.Security.Claims;
using System.Text.Json.Serialization;
using conscoord_api;
using conscoord_api.Controllers;
using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using conscoord_api.Services;
using conscoord_api.Utils;
using Coravel;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var envVars = DotEnv.Read();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = "https://dev-zas6rizyxopiwv2b.us.auth0.com/",
        ValidAudience = "BOZHiKTbFJOrquI2E4QMI2qARqMW9OgC"
    };
    options.Authority = "https://dev-zas6rizyxopiwv2b.us.auth0.com/";
});
builder.Services.AddAuthorization();

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
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ShiftClockInReminder>();
builder.Services.AddScoped<RoleUtils>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Health Probe
app.MapGet("/api/health", () => "healthy");
app.MapGet("/api/auth", (ClaimsPrincipal user) =>
{
    if (user.Identity?.IsAuthenticated == true)
    {
        var authUser = user?.FindFirst(ClaimTypes.Email)?.Value;
        Console.WriteLine($"Authenticated user: {authUser}");

        return $"Authenticated user: {authUser}";
    }

    Console.WriteLine("User not authenticated");
    return "User not authenticated";
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<SendEmailsAtMidnight>()
        .Cron("0 0 * * *")
        .PreventOverlapping(nameof(SendEmailsAtMidnight));

    scheduler.Schedule<ShiftClockInReminder>()
        .Cron("0 0 * * 1")
        .PreventOverlapping(nameof(ShiftClockInReminder));
});

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin());

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
