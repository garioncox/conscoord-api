using conscoord_api.Controllers;
using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using conscoord_api.Services;
using Microsoft.EntityFrameworkCore;
using dotenv.net;

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

// Services
builder.Services.Configure<SmtpSettings>(options =>
{
    options.SenderName = envVars["SMTP_SENDERNAME"];
    options.Username = envVars["SMTP_USERNAME"];
    options.Password = envVars["SMTP_PASSWORD"];
});
builder.Services.AddDbContext<PostgresContext>(options => options.UseNpgsql(envVars["DB"]));
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IEmployeeShiftService, EmployeeShiftService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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