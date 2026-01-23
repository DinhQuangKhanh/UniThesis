using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.DefenseAggregate;
using UniThesis.Domain.Aggregates.DefenseAggregate.Entities;
using UniThesis.Domain.Aggregates.EvaluationAggregate;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Domain.Aggregates.GroupAggregate.Entities;
using UniThesis.Domain.Aggregates.MeetingAggregate;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Aggregates.ProjectAggregate.Entities;
using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Aggregates.SemesterAggregate.Entities;
using UniThesis.Domain.Aggregates.SupportAggregate;
using UniThesis.Domain.Aggregates.TopicPoolAggregate;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Entities;
using UniThesis.Domain.Entities;
using UniThesis.Persistence.SqlServer.Identity;

namespace UniThesis.Persistence.SqlServer;

/// <summary>
/// Main application DbContext with Identity support.
/// </summary>
public class AppDbContext : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    Guid,
    IdentityUserClaim<Guid>,
    ApplicationUserRole,
    IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>,
    IdentityUserToken<Guid>>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    #region Project Aggregate
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMentor> ProjectMentors => Set<ProjectMentor>();
    public DbSet<Document> Documents => Set<Document>();
    #endregion

    #region TopicPool Aggregate
    public DbSet<TopicPool> TopicPools => Set<TopicPool>();
    public DbSet<TopicRegistration> TopicRegistrations => Set<TopicRegistration>();
    #endregion

    #region Group Aggregate
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
    #endregion

    #region Semester Aggregate
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<SemesterPhase> SemesterPhases => Set<SemesterPhase>();
    #endregion

    #region Evaluation Aggregate
    public DbSet<EvaluationSubmission> EvaluationSubmissions => Set<EvaluationSubmission>();
    #endregion

    #region Defense Aggregate
    public DbSet<DefenseSchedule> DefenseSchedules => Set<DefenseSchedule>();
    public DbSet<DefenseCouncil> DefenseCouncils => Set<DefenseCouncil>();
    public DbSet<CouncilMember> CouncilMembers => Set<CouncilMember>();
    #endregion

    #region Meeting Aggregate
    public DbSet<MeetingSchedule> MeetingSchedules => Set<MeetingSchedule>();
    #endregion

    #region Support Aggregate
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    #endregion

    #region Standalone Entities
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Major> Majors => Set<Major>();
    public DbSet<SystemConfiguration> SystemConfigurations => Set<SystemConfiguration>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<ProjectArchive> ProjectArchives => Set<ProjectArchive>();
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Rename Identity tables
        modelBuilder.Entity<ApplicationUser>(b => b.ToTable("Users"));
        modelBuilder.Entity<ApplicationRole>(b => b.ToTable("Roles"));
        modelBuilder.Entity<ApplicationUserRole>(b => b.ToTable("UserRoles"));
        modelBuilder.Entity<IdentityUserClaim<Guid>>(b => b.ToTable("UserClaims"));
        modelBuilder.Entity<IdentityUserLogin<Guid>>(b => b.ToTable("UserLogins"));
        modelBuilder.Entity<IdentityUserToken<Guid>>(b => b.ToTable("UserTokens"));
        modelBuilder.Entity<IdentityRoleClaim<Guid>>(b => b.ToTable("RoleClaims"));
    }

    /// <summary>
    /// Override SaveChangesAsync to handle domain events.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Domain events can be dispatched here or via interceptor
        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }
}