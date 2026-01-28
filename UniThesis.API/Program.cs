using UniThesis.Application;
using UniThesis.Infrastructure;
using UniThesis.Infrastructure.SignalR;
using UniThesis.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1. ADD SERVICES TO THE CONTAINER
// ============================================

// Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "UniThesis API",
        Version = "v1",
        Description = "API for UniThesis - University Thesis Management System"
    });

    // JWT Authentication in Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                ?? new[] { "http://localhost:3000", "http://localhost:5173" })
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Layer Services
builder.Services.AddApplicationServices();  // Application Layer (MediatR, Validators, Behaviors)
builder.Services.AddPersistence(builder.Configuration);  // Persistence Layer (EF Core, MongoDB, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);  // Infrastructure Layer (Auth, Email, Caching, etc.)

var app = builder.Build();

// ============================================
// 2. CONFIGURE THE HTTP REQUEST PIPELINE
// ============================================

// Development-specific middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "UniThesis API v1");
        options.RoutePrefix = "swagger";
    });
}

// Infrastructure middleware (Correlation ID, Request Logging, Exception Handling, Performance Monitoring)
app.UseInfrastructure();

// HTTPS Redirection
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowFrontend");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Health Checks
app.MapHealthChecks("/health");

// SignalR Hubs
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<ChatHub>("/hubs/chat");

// Controllers
app.MapControllers();

app.Run();
