using Microsoft.EntityFrameworkCore;
using UniThesis.Persistence.SqlServer;

namespace UniThesis.Persistence.Seeds;

/// <summary>
/// Seeds development/testing data into the database.
/// Uses raw SQL to bypass domain validation (e.g., Email ValueObject only allows @fpt.edu.vn).
/// Idempotent: checks for existing data before inserting.
/// </summary>
public static class DevelopmentDataSeeder
{
    // ─── Hardcoded GUIDs for consistent cross-table references ───
    private static readonly Guid AdminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid EvaluatorId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private static readonly Guid MentorId = Guid.Parse("00000000-0000-0000-0000-000000000003");
    private static readonly Guid Student1Id = Guid.Parse("00000000-0000-0000-0000-000000000004"); // Khanh
    private static readonly Guid Student2Id = Guid.Parse("00000000-0000-0000-0000-000000000005"); // Chau
    private static readonly Guid Student3Id = Guid.Parse("00000000-0000-0000-0000-000000000006"); // Quynh
    private static readonly Guid Student4Id = Guid.Parse("00000000-0000-0000-0000-000000000007"); // Kien

    private static readonly Guid Group1Id = Guid.Parse("00000000-0000-0000-0001-000000000001");
    private static readonly Guid Group2Id = Guid.Parse("00000000-0000-0000-0001-000000000002");
    private static readonly Guid Project1Id = Guid.Parse("00000000-0000-0000-0002-000000000001");
    private static readonly Guid Project2Id = Guid.Parse("00000000-0000-0000-0002-000000000002");

    private const int SemesterId = 100;
    private static readonly DateTime SeedDate = new(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);

    public static async Task SeedAsync(AppDbContext context)
    {
        // Idempotent check: if admin user already exists, skip seeding
        var alreadySeeded = await context.Database
            .SqlQueryRaw<int>("SELECT COUNT(*) AS [Value] FROM Users WHERE Id = {0}", AdminId)
            .FirstOrDefaultAsync();

        if (alreadySeeded > 0)
            return;

        await SeedUsersAsync(context);
        await SeedUserRolesAsync(context);
        await SeedSemesterAsync(context);
        await SeedGroupsAsync(context);
        await SeedGroupMembersAsync(context);
        await SeedProjectsAsync(context);
        await SeedProjectMentorsAsync(context);
    }

    private static async Task SeedUsersAsync(AppDbContext context)
    {
        var sql = @"
            INSERT INTO Users (Id, Email, FullName, AvatarUrl, StudentCode, EmployeeCode, AcademicTitle, DepartmentId, Status, FirebaseUid, CreatedAt, UpdatedAt, LastLoginAt)
            VALUES
            -- Staff (Gmail accounts)
            (@p0,  'c@gmail.com',                    'Admin Test',           NULL, NULL,       'EMP-001', NULL, 1, 0, 'X4ExMiqyoCeUtNNziNSvYINWCwl1', @p7, NULL, NULL),
            (@p1,  'a@gmail.com',                    'Evaluator Test',       NULL, NULL,       'EMP-002', NULL, 1, 0, 'CccQWlkybaNZJwTjPFHK30F8Jw13', @p7, NULL, NULL),
            (@p2,  'k@gmail.com',                    'Mentor Test',          NULL, NULL,       'EMP-003', NULL, 1, 0, '7PqQE53yimRkJj7WLfDRxNcWiw22', @p7, NULL, NULL),
            -- Students (FPT accounts)
            (@p3,  'khanhdqde170745@fpt.edu.vn',     'Dang Quoc Khanh',     NULL, 'DE170745', NULL,      NULL, 1, 0, 'I1dB9hi0qjZmwMkOgZPNM5x2Ruq2', @p7, NULL, NULL),
            (@p4,  'chaundhde170559@fpt.edu.vn',     'Nguyen Duc Huy Chau', NULL, 'DE170559', NULL,      NULL, 1, 0, 'WmbGM0U9nOfgk1ToSpcYiJzur5i2', @p7, NULL, NULL),
            (@p5,  'quynhldde170026@fpt.edu.vn',     'Le Dang Quynh',       NULL, 'DE170026', NULL,      NULL, 1, 0, 'kKSleFnvFIRchVqka5yI6mHz5452', @p7, NULL, NULL),
            (@p6,  'kienvvde170297@fpt.edu.vn',      'Vu Van Kien',         NULL, 'DE170297', NULL,      NULL, 1, 0, 'VJ1aq0Fj1eNQF5gbUOXNVD8GChS2', @p7, NULL, NULL);";

        await context.Database.ExecuteSqlRawAsync(sql,
            AdminId, EvaluatorId, MentorId,
            Student1Id, Student2Id, Student3Id, Student4Id,
            SeedDate);
    }

    private static async Task SeedUserRolesAsync(AppDbContext context)
    {
        var sql = @"
            INSERT INTO UserRoles (UserId, RoleName, AssignedAt, AssignedBy, IsActive)
            VALUES
            (@p0, 'Admin',     @p4, NULL, 1),
            (@p1, 'Evaluator', @p4, NULL, 1),
            (@p2, 'Mentor',    @p4, NULL, 1),
            (@p3, 'Student',   @p4, NULL, 1),
            (@p5, 'Student',   @p4, NULL, 1),
            (@p6, 'Student',   @p4, NULL, 1),
            (@p7, 'Student',   @p4, NULL, 1);";

        await context.Database.ExecuteSqlRawAsync(sql,
            AdminId, EvaluatorId, MentorId, Student1Id,
            SeedDate,
            Student2Id, Student3Id, Student4Id);
    }

    private static async Task SeedSemesterAsync(AppDbContext context)
    {
        var sql = @"
            INSERT INTO Semesters (Id, Name, Code, StartDate, EndDate, Status, AcademicYear, Description, CreatedAt, UpdatedAt)
            VALUES
            (@p0, 'Spring 2026', 'SP2026', @p1, @p2, 1, '2025-2026', 'Spring semester 2026 - Active', @p3, NULL);";

        await context.Database.ExecuteSqlRawAsync(sql,
            SemesterId,
            new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),  // StartDate
            new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc),  // EndDate
            SeedDate);
    }

    private static async Task SeedGroupsAsync(AppDbContext context)
    {
        var sql = @"
            INSERT INTO Groups (Id, Code, Name, ProjectId, SemesterId, LeaderId, Status, MaxMembers, CreatedAt, UpdatedAt)
            VALUES
            (@p0, 'G-2026-001', 'Team Alpha', NULL, @p2, @p3, 0, 5, @p5, NULL),
            (@p1, 'G-2026-002', 'Team Beta',  NULL, @p2, @p4, 0, 5, @p5, NULL);";

        await context.Database.ExecuteSqlRawAsync(sql,
            Group1Id, Group2Id,
            SemesterId,
            Student1Id, Student3Id,  // Leaders: Khanh, Quynh
            SeedDate);
    }

    private static async Task SeedGroupMembersAsync(AppDbContext context)
    {
        var sql = @"
            INSERT INTO GroupMembers (GroupId, StudentId, Role, Status, JoinedAt, LeftAt)
            VALUES
            -- Group 1: Khanh (Leader) + Chau (Member)
            (@p0, @p2, 0, 0, @p6, NULL),
            (@p0, @p3, 1, 0, @p6, NULL),
            -- Group 2: Quynh (Leader) + Kien (Member)
            (@p1, @p4, 0, 0, @p6, NULL),
            (@p1, @p5, 1, 0, @p6, NULL);";

        await context.Database.ExecuteSqlRawAsync(sql,
            Group1Id, Group2Id,
            Student1Id, Student2Id,  // Group 1: Khanh, Chau
            Student3Id, Student4Id,  // Group 2: Quynh, Kien
            SeedDate);
    }

    private static async Task SeedProjectsAsync(AppDbContext context)
    {
        var sql = @"
            INSERT INTO Projects (Id, Code, NameVi, NameEn, NameAbbr, Description, Objectives, Scope, Technologies, ExpectedResults,
                                  MajorId, SemesterId, GroupId, TopicPoolId, MaxStudents, SourceType, RegistrationType, Status, Priority,
                                  SubmittedAt, SubmittedBy, ApprovedAt, StartDate, Deadline, EvaluationCount, LastEvaluationResult,
                                  PoolStatus, CreatedInSemesterId, ExpirationSemesterId, CreatedAt, UpdatedAt)
            VALUES
            (@p0,  'PROJ-2026-001', N'Hệ thống quản lý đồ án tốt nghiệp', 'Thesis Management System', 'TMS',
             N'Xây dựng hệ thống quản lý đồ án tốt nghiệp cho trường đại học',
             N'Phát triển hệ thống web quản lý toàn bộ quy trình đồ án',
             N'Web application, REST API, Real-time notifications',
             '.NET 8, React, SQL Server, SignalR',
             N'Hệ thống hoàn chỉnh có thể triển khai thực tế',
             1, @p2, @p4, NULL, 5, 1, 0, 2, 0,
             NULL, NULL, NULL, NULL, NULL, 0, NULL,
             NULL, NULL, NULL, @p6, NULL),
            (@p1,  'PROJ-2026-002', N'Ứng dụng AI trong giáo dục', 'AI in Education Application', 'AIED',
             N'Nghiên cứu và phát triển ứng dụng AI hỗ trợ học tập',
             N'Xây dựng chatbot hỗ trợ sinh viên trong học tập',
             N'AI Chatbot, NLP, Knowledge base',
             'Python, FastAPI, React, PostgreSQL, LangChain',
             N'Chatbot có thể trả lời câu hỏi về môn học',
             3, @p2, @p5, NULL, 5, 1, 0, 2, 0,
             NULL, NULL, NULL, NULL, NULL, 0, NULL,
             NULL, NULL, NULL, @p6, NULL);";

        await context.Database.ExecuteSqlRawAsync(sql,
            Project1Id, Project2Id,
            SemesterId,
            MentorId,  // SubmittedBy (not used, but for reference)
            Group1Id, Group2Id,
            SeedDate);

        // Update Groups to reference their Projects
        await context.Database.ExecuteSqlRawAsync(
            "UPDATE Groups SET ProjectId = @p0 WHERE Id = @p1; UPDATE Groups SET ProjectId = @p2 WHERE Id = @p3;",
            Project1Id, Group1Id,
            Project2Id, Group2Id);
    }

    private static async Task SeedProjectMentorsAsync(AppDbContext context)
    {
        var sql = @"
            INSERT INTO ProjectMentors (ProjectId, MentorId, Status, AssignedAt, AssignedBy, Notes)
            VALUES
            (@p0, @p2, 0, @p3, NULL, NULL),
            (@p1, @p2, 0, @p3, NULL, NULL);";

        await context.Database.ExecuteSqlRawAsync(sql,
            Project1Id, Project2Id,
            MentorId,
            SeedDate);
    }
}
