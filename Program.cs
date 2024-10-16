using conscoord_api.Controllers;
using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using conscoord_api.Services;
using Microsoft.EntityFrameworkCore;
using Coravel;
using dotenv.net;
using System.Text.Json.Serialization;

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
//this is how you pass in parameters
//builder.Services.AddTransient<string>(p => "");

// Feature Flags
builder.Services.Configure<FeatureFlags>(o =>
    {
        o.EMAIL_ENABLED = envVars["EMAIL_ENABLED"] == "TRUE";
    }
);

// Services
builder.Services.Configure<SmtpSettings>(options =>
{
    options.SenderName = envVars["SMTP_SENDERNAME"];
    options.Username = envVars["SMTP_USERNAME"];
    options.Password = envVars["SMTP_PASSWORD"];
});
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); // Prevent circular dependencies
builder.Services.AddDbContext<PostgresContext>(options => options.UseNpgsql(envVars["DB"]));
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IProjectShiftService, ProjectShiftService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IEmployeeShiftService, EmployeeShiftService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<EmailController>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

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