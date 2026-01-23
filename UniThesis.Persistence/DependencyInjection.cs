using Microsoft.AspNetCore.Identity;
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
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Persistence.MongoDB;
using UniThesis.Persistence.MongoDB.Indexes;
using UniThesis.Persistence.MongoDB.Repositories.Implementation;
using UniThesis.Persistence.MongoDB.Repositories.Interfaces;
using UniThesis.Persistence.Services;
using UniThesis.Persistence.SqlServer;
using UniThesis.Persistence.SqlServer.Constants;
using UniThesis.Persistence.SqlServer.Identity;
using UniThesis.Persistence.SqlServer.Interceptors;
using UniThesis.Persistence.SqlServer.Repositories;

namespace UniThesis.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Add HttpContextAccessor for CurrentUserService
            services.AddHttpContextAccessor();

            // Add Core Services
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IDateTimeService, DateTimeService>();

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

            // Add Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

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
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ITopicPoolRepository, TopicPoolRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<ISemesterRepository, SemesterRepository>();
            services.AddScoped<IEvaluationSubmissionRepository, EvaluationSubmissionRepository>();
            services.AddScoped<IDefenseScheduleRepository, DefenseScheduleRepository>();
            services.AddScoped<IMeetingScheduleRepository, MeetingScheduleRepository>();
            services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();

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

        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();
            var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
            await MongoIndexConfiguration.CreateIndexesAsync(mongoContext);
        }

        public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            foreach (var roleName in RoleNames.All)
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new ApplicationRole(roleName) { Description = $"System: {roleName}", IsSystemRole = true });

            if (await userManager.FindByEmailAsync("admin@unithesis.edu.vn") is null)
            {
                var admin = new ApplicationUser { UserName = "admin@unithesis.edu.vn", Email = "admin@unithesis.edu.vn", EmailConfirmed = true, FullName = "System Admin", EmployeeCode = "ADMIN001", Status = Domain.Enums.User.UserStatus.Active };
                if ((await userManager.CreateAsync(admin, "Admin@123")).Succeeded) await userManager.AddToRoleAsync(admin, RoleNames.Admin);
            }
        }
    }
}
