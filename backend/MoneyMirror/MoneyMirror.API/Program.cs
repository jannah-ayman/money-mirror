using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoneyMirror.Core.Interfaces;
using MoneyMirror.Infrastructure.Data;
using MoneyMirror.Infrastructure.Services;
using FluentValidation;
using System.Text;
using Hangfire;
using Hangfire.SqlServer;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// ==================== CORS CONFIGURATION ====================
// CORS (Cross-Origin Resource Sharing) allows your React Native app to call the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactNative", policy =>
    {
        policy.WithOrigins("http://localhost:19006", "http://localhost:8081") // Expo and bare RN ports
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ==================== DATABASE CONFIGURATION ====================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

// ==================== HANGFIRE CONFIGURATION ====================
// Hangfire for background jobs (account cleanup, scheduled tasks)
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }
    )
);

// Add Hangfire server
builder.Services.AddHangfireServer();
builder.Services.AddHttpClient();

// ==================== DEPENDENCY INJECTION - REGISTER SERVICES ====================
// These tell ASP.NET Core: "When someone asks for IAuthService, give them AuthService"
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IChildService, ChildService>();
builder.Services.AddScoped<IPersonalityProfileService, PersonalityProfileService>();
builder.Services.AddScoped<IAllowanceService, AllowanceService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IAIPersonalityService, AIPersonalityService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IGoalService, GoalService>();

// ==================== JWT AUTHENTICATION CONFIGURATION ====================
// Read JWT settings from appsettings.json
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer not configured");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT Audience not configured");

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    // Use JWT Bearer tokens as the default authentication scheme
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Configure how JWT tokens are validated
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Verify token signature
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ValidateIssuer = true, // Verify issuer (who created the token)
        ValidIssuer = jwtIssuer,
        ValidateAudience = true, // Verify audience (who token is for)
        ValidAudience = jwtAudience,
        ValidateLifetime = true, // Check if token is expired
        ClockSkew = TimeSpan.Zero // No grace period for expiration
    };

    // Configure events for debugging and logging
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            // Log authentication failures
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            // Log successful token validation
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        }
    };
});

// Add authorization (works with authentication)
builder.Services.AddAuthorization();

// ==================== CONTROLLERS ====================
builder.Services.AddControllers(options =>
{
    // Add FluentValidation filter for automatic validation
    options.Filters.Add<MoneyMirror.API.Filters.FluentValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter()
    );
});


// ==================== FLUENTVALIDATION CONFIGURATION ====================
// Register all FluentValidation validators from the API assembly
// This scans the API project and automatically registers all validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Register the FluentValidation filter as a service
builder.Services.AddScoped<MoneyMirror.API.Filters.FluentValidationFilter>();

// ==================== SWAGGER / OPENAPI CONFIGURATION ====================
// Swagger provides interactive API documentation at /swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Money Mirror API",
        Version = "v1",
        Description = "API for Money Mirror - Financial literacy app for children",
        Contact = new OpenApiContact
        {
            Name = "Money Mirror Team",
            Email = "support@moneymirror.com"
        }
    });

    // Add JWT authentication to Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ==================== BUILD THE APP ====================
var app = builder.Build();

// ==================== MIDDLEWARE PIPELINE ====================
// Middleware runs in order - each request flows through these

// 1. DEVELOPMENT TOOLS
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Money Mirror API v1");
        options.RoutePrefix = "swagger"; // Access at /swagger
    });
}

// 2. HTTPS REDIRECTION (redirect HTTP to HTTPS)
app.UseHttpsRedirection();
app.UseStaticFiles();
// 3. CORS (must come before authentication)
app.UseCors("AllowReactNative");

// 4. AUTHENTICATION (validates JWT tokens)
app.UseAuthentication();

// 5. AUTHORIZATION (checks if user has permission)
app.UseAuthorization();

// 6. HANGFIRE DASHBOARD (optional - for monitoring background jobs)
// Access at: https://localhost:7XXX/hangfire
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

// 7. MAP CONTROLLERS (route requests to controller actions)
app.MapControllers();

// ==================== CONFIGURE BACKGROUND JOBS ====================
// Schedule recurring job to permanently delete expired accounts daily at 3 AM
RecurringJob.AddOrUpdate<IAuthService>(
    "permanent-deletion-job",
    service => service.PermanentlyDeleteExpiredAccountsAsync(),
    Cron.Daily(3)); // Runs daily at 3:00 AM server time

// Schedule recurring job to credit scheduled allowances every hour
RecurringJob.AddOrUpdate<IAllowanceService>(
    "credit-scheduled-allowances",
    service => service.CreditScheduledAllowancesAsync(),
    "*/15 * * * *"); // runs every 15 minutes

// Schedule job to update all children's ages daily at 2 AM
RecurringJob.AddOrUpdate<IChildService>(
    "update-children-ages",
    service => service.UpdateAllChildrenAgesAsync(),
    Cron.Daily(2));
RecurringJob.AddOrUpdate<IGoalService>(
    "fail-expired-goals",
    service => service.FailExpiredGoalsAsync(),
    Cron.Daily(1)); // Runs daily at 1:00 AM

// ==================== RUN THE APPLICATION ====================
Console.WriteLine("Money Mirror API is starting...");
Console.WriteLine("Swagger documentation available at: https://localhost:7XXX/swagger");
Console.WriteLine("Hangfire dashboard available at: https://localhost:7XXX/hangfire");
app.Run();

// ==================== HANGFIRE AUTHORIZATION FILTER ====================
// Simple authorization filter for Hangfire dashboard (development only)
// In production, implement proper authentication
public class HangfireAuthorizationFilter : Hangfire.Dashboard.IDashboardAuthorizationFilter
{
    public bool Authorize(Hangfire.Dashboard.DashboardContext context)
    {
        // For development: allow all access to Hangfire dashboard
        // For production: implement proper authorization (check user roles, etc.)
#if DEBUG
        return true;
#else
            // Production: only allow authenticated admin users
            // You would need to implement user role checking here
            return false;
#endif
    }
}