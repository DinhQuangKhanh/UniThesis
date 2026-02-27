using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniThesis.Persistence.SqlServer;

namespace UniThesis.Persistence.Seeds;

/// <summary>
/// Seeds 1000 load-test users (and related groups, projects, mentors) into the database.
/// Designed to work with the Firebase Auth Emulator — every user has a deterministic
/// FirebaseUid so <see cref="FirebaseEmulatorSeeder"/> can create the matching accounts.
/// Uses raw SQL to bypass domain validation. Idempotent.
/// </summary>
public static class LoadTestDataSeeder
{
    // ────────────────── Distribution ──────────────────
    private const int AdminCount = 5;
    private const int EvaluatorCount = 20;
    private const int MentorCount = 50;
    private const int StudentCount = 925;
    private const int StudentsPerGroup = 4;

    private const int SemesterId = 100; // Reuse DevelopmentDataSeeder semester
    private static readonly DateTime SeedDate = new(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);

    private const int BatchSize = 50;

    // ────────────────── ID helpers ──────────────────
    public static Guid AdminId(int i) => Guid.Parse($"10000000-0000-0000-0000-{i:D12}");
    public static Guid EvaluatorId(int i) => Guid.Parse($"20000000-0000-0000-0000-{i:D12}");
    public static Guid MentorId(int i) => Guid.Parse($"30000000-0000-0000-0000-{i:D12}");
    public static Guid StudentId(int i) => Guid.Parse($"40000000-0000-0000-0000-{i:D12}");
    private static Guid GroupId(int i) => Guid.Parse($"50000000-0000-0000-0000-{i:D12}");
    private static Guid ProjectId(int i) => Guid.Parse($"60000000-0000-0000-0000-{i:D12}");

    public static string AdminFirebaseUid(int i) => $"test-admin-{i:D4}";
    public static string EvaluatorFirebaseUid(int i) => $"test-eval-{i:D4}";
    public static string MentorFirebaseUid(int i) => $"test-mentor-{i:D4}";
    public static string StudentFirebaseUid(int i) => $"test-student-{i:D4}";

    public static string AdminEmail(int i) => $"admin{i}@test.unithesis.dev";
    public static string EvaluatorEmail(int i) => $"evaluator{i}@test.unithesis.dev";
    public static string MentorEmail(int i) => $"mentor{i}@test.unithesis.dev";
    public static string StudentEmail(int i) => $"student{i}@test.unithesis.dev";

    public const string DefaultPassword = "Test@123456";

    // ────────────────── Entry point ──────────────────
    public static async Task SeedAsync(AppDbContext context, ILogger? logger = null)
    {
        var alreadySeeded = await context.Database
            .SqlQueryRaw<int>("SELECT COUNT(*) AS [Value] FROM Users WHERE Id = {0}", AdminId(1))
            .FirstOrDefaultAsync();

        if (alreadySeeded > 0)
        {
            logger?.LogInformation("Load-test data already seeded, skipping.");
            return;
        }

        logger?.LogInformation("Seeding 1000 load-test users...");

        await SeedUsersAsync(context, logger);
        await SeedUserRolesAsync(context, logger);
        await SeedGroupsAsync(context, logger);
        await SeedGroupMembersAsync(context, logger);
        await SeedProjectsAsync(context, logger);
        await SeedProjectMentorsAsync(context, logger);

        logger?.LogInformation("Load-test data seeding complete.");
    }

    // ════════════════════════════════════════════════
    //  USERS
    // ════════════════════════════════════════════════
    private static async Task SeedUsersAsync(AppDbContext context, ILogger? logger)
    {
        var users = new List<(Guid Id, string Email, string FullName, string? StudentCode, string? EmployeeCode, string FirebaseUid)>();

        for (var i = 1; i <= AdminCount; i++)
            users.Add((AdminId(i), AdminEmail(i), $"Admin LoadTest {i}", null, $"LT-EMP-A{i:D3}", AdminFirebaseUid(i)));

        for (var i = 1; i <= EvaluatorCount; i++)
            users.Add((EvaluatorId(i), EvaluatorEmail(i), $"Evaluator LoadTest {i}", null, $"LT-EMP-E{i:D3}", EvaluatorFirebaseUid(i)));

        for (var i = 1; i <= MentorCount; i++)
            users.Add((MentorId(i), MentorEmail(i), $"Mentor LoadTest {i}", null, $"LT-EMP-M{i:D3}", MentorFirebaseUid(i)));

        for (var i = 1; i <= StudentCount; i++)
            users.Add((StudentId(i), StudentEmail(i), $"Student LoadTest {i}", $"LT-{i:D6}", null, StudentFirebaseUid(i)));

        // Batch insert
        for (var batch = 0; batch < users.Count; batch += BatchSize)
        {
            var chunk = users.Skip(batch).Take(BatchSize).ToList();
            var valueClauses = new List<string>();
            var parameters = new List<object>();
            var paramIndex = 0;

            foreach (var u in chunk)
            {
                var pId = $"@p{paramIndex++}";
                var pEmail = $"@p{paramIndex++}";
                var pName = $"@p{paramIndex++}";
                var pStudentCode = $"@p{paramIndex++}";
                var pEmployeeCode = $"@p{paramIndex++}";
                var pFirebaseUid = $"@p{paramIndex++}";
                var pDate = $"@p{paramIndex++}";

                valueClauses.Add($"({pId}, {pEmail}, {pName}, NULL, {pStudentCode}, {pEmployeeCode}, NULL, 1, 0, {pFirebaseUid}, {pDate}, NULL, NULL)");

                parameters.Add(u.Id);
                parameters.Add(u.Email);
                parameters.Add(u.FullName);
                parameters.Add((object?)u.StudentCode);  // null sẽ tự convert thành SQL NULL
                parameters.Add((object?)u.EmployeeCode); // null sẽ tự convert thành SQL NULL
                parameters.Add(u.FirebaseUid);
                parameters.Add(SeedDate);
            }

            var sql = $@"
                INSERT INTO Users (Id, Email, FullName, AvatarUrl, StudentCode, EmployeeCode, AcademicTitle, DepartmentId, Status, FirebaseUid, CreatedAt, UpdatedAt, LastLoginAt)
                VALUES {string.Join(",\n                       ", valueClauses)};";

            await context.Database.ExecuteSqlRawAsync(sql, parameters.ToArray());
        }

        logger?.LogInformation("Seeded {Count} load-test users.", users.Count);
    }

    // ════════════════════════════════════════════════
    //  USER ROLES
    // ════════════════════════════════════════════════
    private static async Task SeedUserRolesAsync(AppDbContext context, ILogger? logger)
    {
        var roles = new List<(Guid UserId, string RoleName)>();

        for (var i = 1; i <= AdminCount; i++)
            roles.Add((AdminId(i), "Admin"));
        for (var i = 1; i <= EvaluatorCount; i++)
            roles.Add((EvaluatorId(i), "Evaluator"));
        for (var i = 1; i <= MentorCount; i++)
            roles.Add((MentorId(i), "Mentor"));
        for (var i = 1; i <= StudentCount; i++)
            roles.Add((StudentId(i), "Student"));

        for (var batch = 0; batch < roles.Count; batch += BatchSize)
        {
            var chunk = roles.Skip(batch).Take(BatchSize).ToList();
            var valueClauses = new List<string>();
            var parameters = new List<object>();
            var paramIndex = 0;

            foreach (var r in chunk)
            {
                var pUserId = $"@p{paramIndex++}";
                var pRole = $"@p{paramIndex++}";
                var pDate = $"@p{paramIndex++}";

                valueClauses.Add($"({pUserId}, {pRole}, {pDate}, NULL, 1)");
                parameters.Add(r.UserId);
                parameters.Add(r.RoleName);
                parameters.Add(SeedDate);
            }

            var sql = $@"
                INSERT INTO UserRoles (UserId, RoleName, AssignedAt, AssignedBy, IsActive)
                VALUES {string.Join(",\n                       ", valueClauses)};";

            await context.Database.ExecuteSqlRawAsync(sql, parameters.ToArray());
        }

        logger?.LogInformation("Seeded {Count} load-test user roles.", roles.Count);
    }

    // ════════════════════════════════════════════════
    //  GROUPS  (925 students / 4 = 231 full groups + 1 partial)
    // ════════════════════════════════════════════════
    private static async Task SeedGroupsAsync(AppDbContext context, ILogger? logger)
    {
        var groupCount = (int)Math.Ceiling((double)StudentCount / StudentsPerGroup); // 232

        for (var batch = 0; batch < groupCount; batch += BatchSize)
        {
            var end = Math.Min(batch + BatchSize, groupCount);
            var valueClauses = new List<string>();
            var parameters = new List<object>();
            var paramIndex = 0;

            for (var i = batch + 1; i <= end; i++)
            {
                var leaderId = StudentId((i - 1) * StudentsPerGroup + 1); // First student in group is leader
                var pId = $"@p{paramIndex++}";
                var pCode = $"@p{paramIndex++}";
                var pName = $"@p{paramIndex++}";
                var pSemester = $"@p{paramIndex++}";
                var pLeader = $"@p{paramIndex++}";
                var pDate = $"@p{paramIndex++}";

                valueClauses.Add($"({pId}, {pCode}, {pName}, NULL, {pSemester}, {pLeader}, 0, 5, {pDate}, NULL)");
                parameters.Add(GroupId(i));
                parameters.Add($"LT-G-{i:D4}");
                parameters.Add($"LoadTest Team {i}");
                parameters.Add(SemesterId);
                parameters.Add(leaderId);
                parameters.Add(SeedDate);
            }

            var sql = $@"
                INSERT INTO Groups (Id, Code, Name, ProjectId, SemesterId, LeaderId, Status, MaxMembers, CreatedAt, UpdatedAt)
                VALUES {string.Join(",\n                       ", valueClauses)};";

            await context.Database.ExecuteSqlRawAsync(sql, parameters.ToArray());
        }

        var totalGroups = (int)Math.Ceiling((double)StudentCount / StudentsPerGroup);
        logger?.LogInformation("Seeded {Count} load-test groups.", totalGroups);
    }

    // ════════════════════════════════════════════════
    //  GROUP MEMBERS
    // ════════════════════════════════════════════════
    private static async Task SeedGroupMembersAsync(AppDbContext context, ILogger? logger)
    {
        var members = new List<(Guid GroupId, Guid StudentId, int Role)>(); // Role: 0=Leader, 1=Member

        for (var s = 1; s <= StudentCount; s++)
        {
            var currentGroup = (s - 1) / StudentsPerGroup + 1;
            var isLeader = (s - 1) % StudentsPerGroup == 0;
            members.Add((GroupId(currentGroup), StudentId(s), isLeader ? 0 : 1));
        }

        for (var batch = 0; batch < members.Count; batch += BatchSize)
        {
            var chunk = members.Skip(batch).Take(BatchSize).ToList();
            var valueClauses = new List<string>();
            var parameters = new List<object>();
            var paramIndex = 0;

            foreach (var m in chunk)
            {
                var pGroup = $"@p{paramIndex++}";
                var pStudent = $"@p{paramIndex++}";
                var pRole = $"@p{paramIndex++}";
                var pDate = $"@p{paramIndex++}";

                valueClauses.Add($"({pGroup}, {pStudent}, {pRole}, 0, {pDate}, NULL)");
                parameters.Add(m.GroupId);
                parameters.Add(m.StudentId);
                parameters.Add(m.Role);
                parameters.Add(SeedDate);
            }

            var sql = $@"
                INSERT INTO GroupMembers (GroupId, StudentId, Role, Status, JoinedAt, LeftAt)
                VALUES {string.Join(",\n                       ", valueClauses)};";

            await context.Database.ExecuteSqlRawAsync(sql, parameters.ToArray());
        }

        logger?.LogInformation("Seeded {Count} load-test group members.", members.Count);
    }

    // ════════════════════════════════════════════════
    //  PROJECTS  (1 per group, mentor assigned round-robin)
    // ════════════════════════════════════════════════
    private static async Task SeedProjectsAsync(AppDbContext context, ILogger? logger)
    {
        var groupCount = (int)Math.Ceiling((double)StudentCount / StudentsPerGroup);
        int[] majorIds = [1, 3]; // Reuse MajorIds from DevelopmentDataSeeder

        for (var batch = 0; batch < groupCount; batch += BatchSize)
        {
            var end = Math.Min(batch + BatchSize, groupCount);
            var valueClauses = new List<string>();
            var parameters = new List<object>();
            var paramIndex = 0;

            for (var i = batch + 1; i <= end; i++)
            {
                var majorId = majorIds[(i - 1) % majorIds.Length];
                var pId = $"@p{paramIndex++}";
                var pCode = $"@p{paramIndex++}";
                var pNameVi = $"@p{paramIndex++}";
                var pNameEn = $"@p{paramIndex++}";
                var pNameAbbr = $"@p{paramIndex++}";  // ← Add NameAbbr parameter
                var pDesc = $"@p{paramIndex++}";
                var pObj = $"@p{paramIndex++}";
                var pMajor = $"@p{paramIndex++}";
                var pSemester = $"@p{paramIndex++}";
                var pGroup = $"@p{paramIndex++}";
                var pDate = $"@p{paramIndex++}";

                valueClauses.Add($@"({pId}, {pCode}, {pNameVi}, {pNameEn}, {pNameAbbr},
                    {pDesc}, {pObj}, NULL, NULL, NULL,
                    {pMajor}, {pSemester}, {pGroup}, NULL, 5, 1, 0, 2, 0,
                    NULL, NULL, NULL, NULL, NULL, 0, NULL,
                    NULL, NULL, NULL, {pDate}, NULL)");

                parameters.Add(ProjectId(i));
                parameters.Add($"LT-PROJ-{i:D4}");
                parameters.Add($"Du an load test {i}");
                parameters.Add($"Load Test Project {i}");
                parameters.Add($"LT-P{i:D4}");  // ← Add NameAbbr value
                parameters.Add($"Mo ta du an load test so {i}");
                parameters.Add($"Muc tieu du an load test so {i}");
                parameters.Add(majorId);
                parameters.Add(SemesterId);
                parameters.Add(GroupId(i));
                parameters.Add(SeedDate);
            }

            var sql = $@"
                INSERT INTO Projects (Id, Code, NameVi, NameEn, NameAbbr,
                    Description, Objectives, Scope, Technologies, ExpectedResults,
                    MajorId, SemesterId, GroupId, TopicPoolId, MaxStudents, SourceType, RegistrationType, Status, Priority,
                    SubmittedAt, SubmittedBy, ApprovedAt, StartDate, Deadline, EvaluationCount, LastEvaluationResult,
                    PoolStatus, CreatedInSemesterId, ExpirationSemesterId, CreatedAt, UpdatedAt)
                VALUES {string.Join(",\n                       ", valueClauses)};";

            await context.Database.ExecuteSqlRawAsync(sql, parameters.ToArray());
        }

        // Update Groups to reference their Projects
        for (var i = 1; i <= groupCount; i++)
        {
            await context.Database.ExecuteSqlRawAsync(
                "UPDATE Groups SET ProjectId = @p0 WHERE Id = @p1;",
                ProjectId(i), GroupId(i));
        }

        logger?.LogInformation("Seeded {Count} load-test projects.", groupCount);
    }

    // ════════════════════════════════════════════════
    //  PROJECT MENTORS  (round-robin from 50 mentors)
    // ════════════════════════════════════════════════
    private static async Task SeedProjectMentorsAsync(AppDbContext context, ILogger? logger)
    {
        var groupCount = (int)Math.Ceiling((double)StudentCount / StudentsPerGroup);

        for (var batch = 0; batch < groupCount; batch += BatchSize)
        {
            var end = Math.Min(batch + BatchSize, groupCount);
            var valueClauses = new List<string>();
            var parameters = new List<object>();
            var paramIndex = 0;

            for (var i = batch + 1; i <= end; i++)
            {
                var mentorIndex = ((i - 1) % MentorCount) + 1; // round-robin 1..50
                var pProject = $"@p{paramIndex++}";
                var pMentor = $"@p{paramIndex++}";
                var pDate = $"@p{paramIndex++}";

                valueClauses.Add($"({pProject}, {pMentor}, 0, {pDate}, NULL, NULL)");
                parameters.Add(ProjectId(i));
                parameters.Add(MentorId(mentorIndex));
                parameters.Add(SeedDate);
            }

            var sql = $@"
                INSERT INTO ProjectMentors (ProjectId, MentorId, Status, AssignedAt, AssignedBy, Notes)
                VALUES {string.Join(",\n                       ", valueClauses)};";

            await context.Database.ExecuteSqlRawAsync(sql, parameters.ToArray());
        }

        logger?.LogInformation("Seeded {Count} load-test project mentors.", groupCount);
    }
}
