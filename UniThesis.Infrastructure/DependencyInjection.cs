using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UniThesis.Domain.Services;
using UniThesis.Infrastructure.Authentication;
using UniThesis.Infrastructure.Authorization;
using UniThesis.Infrastructure.Authorization.Policies;
using UniThesis.Infrastructure.BackgroundJobs;
using UniThesis.Infrastructure.BackgroundJobs.Jobs;
using UniThesis.Infrastructure.BackgroundJobs.Scheduling;
using UniThesis.Infrastructure.Caching;
using UniThesis.Infrastructure.HealthChecks;
using UniThesis.Infrastructure.Middleware;
using UniThesis.Infrastructure.Services.DomainServices;
using UniThesis.Infrastructure.Services.Email;
using UniThesis.Infrastructure.Services.Email.Templates;
using UniThesis.Infrastructure.Services.FileStorage;
using UniThesis.Infrastructure.Services.Notification;
using UniThesis.Infrastructure.Services.Reporting;
using UniThesis.Infrastructure.SignalR;

namespace UniThesis.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // MediatR - Auto-discover all handlers from this assembly
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            // JWT Authentication
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                            context.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
            });

            // Authorization
            services.AddAuthorizationPolicies();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, ProjectOwnerAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, GroupMemberAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, MentorOfProjectAuthorizationHandler>();

            // Token Services
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            // Domain Services
            services.AddScoped<IProjectDomainService, ProjectDomainService>();
            services.AddScoped<IEvaluationDomainService, EvaluationDomainService>();
            services.AddScoped<ITopicPoolDomainService, TopicPoolDomainService>();
            services.AddScoped<ISemesterDomainService, SemesterDomainService>();
            services.AddScoped<IGroupDomainService, GroupDomainService>();

            // Email
            services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();

            // File Storage
            services.Configure<FileStorageSettings>(configuration.GetSection(FileStorageSettings.SectionName));
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<IFileValidationService, FileValidationService>();

            // Notification
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IHubNotificationService, SignalRNotificationService>();

            // Reporting
            services.AddScoped<IReportGeneratorService, ExcelReportService>();

            // Caching
            services.Configure<CacheSettings>(configuration.GetSection(CacheSettings.SectionName));
            services.AddMemoryCache();
            services.AddScoped<ICacheService, MemoryCacheService>();
            services.AddScoped<ICacheInvalidationService, CacheInvalidationService>();

            // Background Jobs
            services.AddScoped<IBackgroundJobService, HangfireJobService>();
            services.AddScoped<TopicExpirationJob>();
            services.AddScoped<EvaluationReminderJob>();
            services.AddScoped<SemesterPhaseTransitionJob>();
            services.AddScoped<DefenseScheduleReminderJob>();
            services.AddScoped<MeetingReminderJob>();
            services.AddScoped<DataCleanupJob>();

            var hangfireConn = configuration.GetConnectionString("HangfireConnection") ?? configuration.GetConnectionString("DefaultConnection");
            services.AddHangfire(c => c.SetDataCompatibilityLevel(CompatibilityLevel.Version_180).UseSimpleAssemblyNameTypeSerializer().UseRecommendedSerializerSettings().UseSqlServerStorage(hangfireConn));
            services.AddHangfireServer();

            // SignalR
            services.AddSignalR();

            // Health Checks
            services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("sqlserver").AddCheck<MongoDbHealthCheck>("mongodb");

            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<PerformanceMonitoringMiddleware>();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new[] { new HangfireAuthFilter() } });
            RecurringJobsConfiguration.ConfigureRecurringJobs();
            return app;
        }
    }

    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.Identity?.IsAuthenticated == true && httpContext.User.IsInRole("Admin");
        }
    }
}
