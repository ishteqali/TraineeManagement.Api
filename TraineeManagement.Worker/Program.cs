using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using TraineeManagement.Shared.Configurations;
using TraineeManagement.Worker;
using Microsoft.Extensions.Options;
using TraineeManagement.Shared.Data;
using TraineeManagement.Worker.Services;
using TraineeManagement.Worker.Interfaces;

Env.Load();

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.SectionName));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));

builder.Services.AddHostedService<Worker>();

builder.Services.AddScoped<ISubmissionProcessorService, SubmissionProcessorService>();

// Configuring the database connection string and adding the DbContext
DatabaseOptions databaseOptions = builder.Configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
    ?? throw new InvalidOperationException("Database configuration is missing.");
string connectionString = $"Server={databaseOptions.Host};Port={databaseOptions.Port};Database={databaseOptions.Database};User={databaseOptions.User};Password={databaseOptions.Password};";

builder.Services.AddDbContext<AppDbContext>(options => options.UseMySQL(connectionString));

IHost host = builder.Build();

host.Run();