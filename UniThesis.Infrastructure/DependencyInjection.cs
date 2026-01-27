using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UniThesis.Domain.Aggregates.DefenseAggregate.Events;
using UniThesis.Domain.Aggregates.EvaluationAggregate.Events;
using UniThesis.Domain.Aggregates.GroupAggregate.Events;
using UniThesis.Domain.Aggregates.MeetingAggregate.Events;
using UniThesis.Domain.Aggregates.ProjectAggregate.Events;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Events;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Services;
using UniThesis.Infrastructure.Authentication;
using UniThesis.Infrastructure.Authorization;
using UniThesis.Infrastructure.Authorization.Policies;
using UniThesis.Infrastructure.BackgroundJobs;
using UniThesis.Infrastructure.BackgroundJobs.Jobs;
using UniThesis.Infrastructure.BackgroundJobs.Scheduling;
using UniThesis.Infrastructure.Caching;
using UniThesis.Infrastructure.EventHandlers.Defense;
using UniThesis.Infrastructure.EventHandlers.Evaluation;
using UniThesis.Infrastructure.EventHandlers.Group;
using UniThesis.Infrastructure.EventHandlers.Meeting;
using UniThesis.Infrastructure.EventHandlers.Project;
using UniThesis.Infrastructure.EventHandlers.TopicPool;
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

            // Event Handlers
            services.AddScoped<IDomainEventHandler<ProjectCreatedEvent>, ProjectCreatedEventHandler>();
            services.AddScoped<IDomainEventHandler<ProjectSubmittedEvent>, ProjectSubmittedEventHandler>();
            services.AddScoped<IDomainEventHandler<ProjectApprovedEvent>, ProjectApprovedEventHandler>();
            services.AddScoped<IDomainEventHandler<ProjectRejectedEvent>, ProjectRejectedEventHandler>();
            services.AddScoped<IDomainEventHandler<EvaluatorAssignedEvent>, EvaluationAssignedEventHandler>();
            services.AddScoped<IDomainEventHandler<EvaluationCompletedEvent>, EvaluationCompletedEventHandler>();
            services.AddScoped<IDomainEventHandler<GroupCreatedEvent>, GroupCreatedEventHandler>();
            services.AddScoped<IDomainEventHandler<MemberAddedEvent>, MemberAddedEventHandler>();
            services.AddScoped<IDomainEventHandler<MeetingRequestedEvent>, MeetingRequestedEventHandler>();
            services.AddScoped<IDomainEventHandler<MeetingApprovedEvent>, MeetingApprovedEventHandler>();
            services.AddScoped<IDomainEventHandler<DefenseScheduledEvent>, DefenseScheduledEventHandler>();
            services.AddScoped<IDomainEventHandler<DefenseCompletedEvent>, DefenseCompletedEventHandler>();

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
