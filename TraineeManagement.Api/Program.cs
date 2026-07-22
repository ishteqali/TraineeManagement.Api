using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Services;
using TraineeManagement.Shared.Data;
using TraineeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog; // for logging
using dotenv.net;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Middleware;
using TraineeManagement.Api.Configurations;
using Microsoft.AspNetCore.Http.Features;
using StackExchange.Redis;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using TraineeManagement.Shared.Configurations;
using TraineeManagement.Api.Configuration;
using Microsoft.Extensions.Http.Resilience;
using System.Net.Mime;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Loading .env varaibles
DotEnv.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.SectionName));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection(FileStorageOptions.SectionName));
builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection(RedisOptions.SectionName));
builder.Services.Configure<TrainingDirectorySettings>(builder.Configuration.GetSection(TrainingDirectorySettings.SectionName));

// Removing Upload file limit for kestral 
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue; // Removes multipart limit
    options.ValueLengthLimit = int.MaxValue;          // Removes single form value limit
    options.MemoryBufferThreshold = int.MaxValue;     // Optional memory buffer bump
});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue; // Removes total request size cap
});

// Configuring the database connection string and adding the DbContext
DatabaseOptions databaseOptions = builder.Configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
    ?? throw new InvalidOperationException("Database configuration is missing.");
string connectionString = $"Server={databaseOptions.Host};Port={databaseOptions.Port};Database={databaseOptions.Database};User={databaseOptions.User};Password={databaseOptions.Password};";

builder.Services.AddDbContext<AppDbContext>(options => options.UseMySQL(connectionString));

// Configuring CORS for ReactFrontend (Phase 3)
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontendPolicy", policy =>
    {
        policy.WithOrigins(
            "https://localhost:3000",
            "https://localhost:5173",
            "https://localhost:5119",
            "https://127.0.0.1:5119")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Configuring Json serialization to handle enums as strings
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}).ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var secureErrorPayload = new
            {
                message = "The request payload format is invalid or malformed."
            };

            return new BadRequestObjectResult(secureErrorPayload);
        };
    }); ;

// Configuring JWT authentication
JwtSettings jwt = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT configuration missing");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwt.Key ?? throw new InvalidOperationException("JWT Key is missing from configuration.")))
        };
    });

// Configuring Serilog for Logging 
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("Logs/api-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add a Security Scheme (using a JWT Bearer token).
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter token"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

RedisOptions redis = builder.Configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>()
    ?? throw new InvalidOperationException("Redis configuration missing.");

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redis.ConnectionString;
    options.InstanceName = redis.InstanceName;
});

IHttpClientBuilder? httpClientBuilder = builder.Services.AddHttpClient<ITrainingDirectoryClient, TrainingDirectoryClient>(client =>
{
    TrainingDirectorySettings trainingDirectorySettings = builder.Configuration.GetSection(TrainingDirectorySettings.SectionName).Get<TrainingDirectorySettings>()
        ?? throw new InvalidOperationException("Training Directory configuration missing.");

    client.BaseAddress = new Uri(trainingDirectorySettings!.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(5);
    client.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
});
httpClientBuilder.AddStandardResilienceHandler();

builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddScoped<ILearningTaskService, LearningTaskService>();
builder.Services.AddScoped<ITaskAssignmentService, TaskAssignmentService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<ISubmissionFileService, SubmissionFileService>();
builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddScoped<IProcessingJobService, ProcessingJobService>();


WebApplication? app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


string? redisConnection = redis.ConnectionString;

try
{
    if (redisConnection != null)
    {
        ConnectionMultiplexer? connection = await ConnectionMultiplexer.ConnectAsync(redisConnection);

        if (connection.IsConnected)
        {
            app.Logger.LogInformation("Redis connected successfully.");
        }
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Unable to connect to Redis.");
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("ReactFrontendPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
