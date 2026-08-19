using DotNetEnv;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using spm_backend.Data;
using spm_backend.Validators;

namespace spm_backend;

public class Program
{
    public static void Main(string[] args)
    {
        Env.Load();
        
        var builder = WebApplication.CreateBuilder(args);

        string connectionTemplate = builder.Configuration.GetConnectionString("DefaultConnection")!;
        
        string connectionString = connectionTemplate
            .Replace("{DB_HOST}", Environment.GetEnvironmentVariable("DB_HOST"))
            .Replace("{DB_PORT}", Environment.GetEnvironmentVariable("DB_PORT"))
            .Replace("{DB_NAME}", Environment.GetEnvironmentVariable("DB_NAME"))
            .Replace("{DB_USER}", Environment.GetEnvironmentVariable("DB_USER"))
            .Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD"));
        
        // Add services to the container.
        builder.Services.AddAuthorization();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        
        builder.Services.AddControllers(option=> option.ModelValidatorProviders.Clear());
        
        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        
        builder.Services.AddValidatorsFromAssemblyContaining<CreateRoleValidator>();
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.MapGet("/", () => Results.Redirect("scalar"));

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}