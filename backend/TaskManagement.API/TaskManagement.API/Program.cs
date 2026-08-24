using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Text.Json;
using TaskManagement.API.Configurations;
using TaskManagement.API.DependencyInjection;
using TaskManagement.API.Extensions;
 
using TaskManagement.Application.DependencyInjection;
using TaskManagement.Infrastructure.DependencyInjection;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Persistence.Contexts;
using TaskManagement.Infrastructure.Persistence.Seed;
using TaskManagement.Infrastructure.Email.Models;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});
//health check
builder.Services.AddApplicationHealthChecks();
// Dependency Injection
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddPresentation();
//setting 
//builder.Services.Configure<JwtSettings>(
//    builder.Configuration.GetSection(JwtSettings.SectionName));

//builder.Services.Configure<EmailSettings>(
//    builder.Configuration.GetSection(EmailSettings.SectionName));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
        options.RoutePrefix = string.Empty;
    });
}

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}
app.UseGlobalExceptionMiddleware();
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseRouting();
app.UseCors("AllowReactApp");


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

// Health Check Endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            Status = report.Status.ToString(),
            Checks = report.Entries.Select(x => new
            {
                Name = x.Key,
                Status = x.Value.Status.ToString(),
                
            })
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
});

app.Run();