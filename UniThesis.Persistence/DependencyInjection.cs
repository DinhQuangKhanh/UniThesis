using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UniThesis.Domain.Aggregates.DefenseAggregate;
using UniThesis.Domain.Aggregates.EvaluationAggregate;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Domain.Aggregates.MeetingAggregate;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Aggregates.SupportAggregate;
using UniThesis.Domain.Aggregates.TopicPoolAggregate;
using UniThesis.Domain.Aggregates.UserAggregate;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Persistence.MongoDB;
using UniThesis.Persistence.MongoDB.Indexes;
using UniThesis.Persistence.MongoDB.Repositories.Implementation;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;
using UniThesis.Persistence.MongoDB.Serializers;
using UniThesis.Persistence.Seeds;
using UniThesis.Persistence.Services;
using UniThesis.Persistence.SqlServer;
using UniThesis.Persistence.SqlServer.Interceptors;
using UniThesis.Persistence.SqlServer.QueryServices;
using UniThesis.Persistence.SqlServer.Repositories;

namespace UniThesis.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure MongoDB serializers (thread-safe, idempotent)
            MongoSerializerConfiguration.Configure();
            // Add HttpContextAccessor for CurrentUserService
            services.AddHttpContextAccessor();

            // Add Core Services
            services.AddScoped<Application.Common.Interfaces.ICurrentUserService, CurrentUserService>();
            services.AddSingleton<Application.Common.Interfaces.IDateTimeService, DateTimeService>();

            // Add Interceptors
            services.AddScoped<AuditableEntityInterceptor>();
            services.AddScoped<SoftDeleteInterceptor>();
            services.AddScoped<DomainEventInterceptor>();

            // Add SQL Server DbContext
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => { b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName); b.EnableRetryOnFailure(3); });
                options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>(),
                    sp.GetRequiredService<SoftDeleteInterceptor>(), sp.GetRequiredService<DomainEventInterceptor>());
            });

            // Add MongoDB
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            services.AddSingleton<MongoDbContext>();

            // Add Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add SQL Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ITopicPoolRepository, TopicPoolRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<ISemesterRepository, SemesterRepository>();
            services.AddScoped<IEvaluationSubmissionRepository, EvaluationSubmissionRepository>();
            services.AddScoped<IDefenseScheduleRepository, DefenseScheduleRepository>();
            services.AddScoped<IMeetingScheduleRepository, MeetingScheduleRepository>();
            services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
            services.AddScoped<ITopicRegistrationRepository, TopicRegistrationRepository>();

            // Add Query Services
            services.AddScoped<Application.Common.Interfaces.IStudentGroupQueryService, StudentGroupQueryService>();

            // Add MongoDB Repositories
            services.AddScoped<IEvaluationLogRepository, EvaluationLogRepository>();
            services.AddScoped<IProjectModificationHistoryRepository, ProjectModificationHistoryRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IUserActivityLogRepository, UserActivityLogRepository>();
            services.AddScoped<ISystemAuditLogRepository, SystemAuditLogRepository>();

            return services;
        }

        /// <summary>
        /// Initializes the database with migrations and seeding.
        /// Call this method after building the WebApplication instance.
        /// </summary>
        /// <example>
        /// var app = builder.Build();
        /// await app.Services.InitializeDatabaseAsync();
        /// </example>
        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();

            // Seed development data (idempotent - skips if data already exists)
            await DevelopmentDataSeeder.SeedAsync(dbContext);

            var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
            await MongoIndexConfiguration.CreateIndexesAsync(mongoContext);
        }
    }
}
