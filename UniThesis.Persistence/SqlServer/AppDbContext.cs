using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.EvaluationAggregate;
using UniThesis.Domain.Aggregates.EvaluationAggregate.Entities;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Domain.Aggregates.GroupAggregate.Entities;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Aggregates.ProjectAggregate.Entities;
using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Aggregates.SemesterAggregate.Entities;
using UniThesis.Domain.Aggregates.SupportAggregate;
using UniThesis.Domain.Aggregates.TopicPoolAggregate;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Entities;
using UniThesis.Domain.Aggregates.UserAggregate;
using UniThesis.Domain.Aggregates.UserAggregate.Entities;
using UniThesis.Domain.Entities;

namespace UniThesis.Persistence.SqlServer;

/// <summary>
/// Main application DbContext without ASP.NET Identity.
/// Uses custom User aggregate with Firebase Authentication.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    #region User Aggregate
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    #endregion

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
    public DbSet<GroupInvitation> GroupInvitations => Set<GroupInvitation>();
    public DbSet<GroupJoinRequest> GroupJoinRequests => Set<GroupJoinRequest>();
    #endregion

    #region Semester Aggregate
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<SemesterPhase> SemesterPhases => Set<SemesterPhase>();
    public DbSet<EligibleStudent> EligibleStudents => Set<EligibleStudent>();
    #endregion

    #region Evaluation Aggregate
    public DbSet<EvaluationSubmission> EvaluationSubmissions => Set<EvaluationSubmission>();
    public DbSet<ProjectEvaluatorAssignment> ProjectEvaluatorAssignments => Set<ProjectEvaluatorAssignment>();
    #endregion

    #region Support Aggregate
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    #endregion

    #region Standalone Entities
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Major> Majors => Set<Major>();
    public DbSet<SystemConfiguration> SystemConfigurations => Set<SystemConfiguration>();
    public DbSet<ProjectArchive> ProjectArchives => Set<ProjectArchive>();
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    // Domain events are dispatched via DomainEventInterceptor
}
