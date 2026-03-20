using Microsoft.EntityFrameworkCore;
using UniThesis.Persistence.SqlServer;

namespace UniThesis.Persistence.Seeds;

/// <summary>
/// Seeds development/testing data into the database.
/// Idempotent: checks for existing data before inserting.
/// </summary>
public static class DevelopmentDataSeeder
{
    // Department
    private const int DeptCNTT = 1; // Khoa Công nghệ thông tin

    // Majors (all belong to DeptCNTT)
    private const int MajorSE = 1; // Kỹ thuật phần mềm
    private const int MajorAI = 2; // Trí tuệ nhân tạo
    private const int MajorDS = 3; // Khoa học dữ liệu ứng dụng
    private const int MajorIA = 4; // An toàn thông tin
    private const int MajorIC = 5; // Vi mạch bán dẫn
    private const int MajorAS = 6; // Công nghệ ô tô số
    private const int MajorIS = 7; // Hệ thống thông tin
    private const int MajorGD = 8; // Thiết kế đồ hoạ và mỹ thuật số

    private const int SemesterId = 100;
    private static readonly DateTime SeedDate = new(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);

    // ─────────────────────────────────────────────────────────────────────────

    public static async Task SeedAsync(AppDbContext context)
    {
        // Idempotent: skip entirely if the department row already exists
        var alreadySeeded = await context.Database
            .SqlQueryRaw<int>("SELECT COUNT(*) AS [Value] FROM Departments WHERE Id = {0}", DeptCNTT)
            .SingleOrDefaultAsync();

        if (alreadySeeded > 0)
            return;

        await SeedDepartmentsAsync(context);
        await SeedMajorsAsync(context);
        await SeedSemestersAsync(context);
        await SeedUsersAsync(context);
        await SeedGroupsAsync(context);
    }

    // ─── 1. Departments ──────────────────────────────────────────────────────

    private static async Task SeedDepartmentsAsync(AppDbContext context)
    {
        var sql = @"
            SET IDENTITY_INSERT Departments ON;
            INSERT INTO Departments (Id, Name, Code, Description, HeadOfDepartmentId, IsActive, CreatedAt, UpdatedAt)
            VALUES (@p0, N'Công nghệ thông tin', 'CNTT', N'Khoa Công nghệ thông tin', NULL, 1, @p1, NULL);
            SET IDENTITY_INSERT Departments OFF;";

        await context.Database.ExecuteSqlRawAsync(sql, DeptCNTT, SeedDate);
    }

    // ─── 2. Majors ───────────────────────────────────────────────────────────

    private static async Task SeedMajorsAsync(AppDbContext context)
    {
        var sql = @"
            SET IDENTITY_INSERT Majors ON;
            INSERT INTO Majors (Id, DepartmentId, Name, Code, Description, IsActive, CreatedAt, UpdatedAt)
            VALUES
            (@p0, @p8, N'Kỹ thuật phần mềm',              'SE',  N'Chuyên ngành Kỹ thuật phần mềm',              1, @p9, NULL),
            (@p1, @p8, N'Trí tuệ nhân tạo',               'AI',  N'Chuyên ngành Trí tuệ nhân tạo',               1, @p9, NULL),
            (@p2, @p8, N'Khoa học dữ liệu ứng dụng',      'DS',  N'Chuyên ngành Khoa học dữ liệu ứng dụng',      1, @p9, NULL),
            (@p3, @p8, N'An toàn thông tin',               'IA',  N'Chuyên ngành An toàn thông tin',              1, @p9, NULL),
            (@p4, @p8, N'Vi mạch bán dẫn',                'IC',  N'Chuyên ngành Vi mạch bán dẫn',                1, @p9, NULL),
            (@p5, @p8, N'Công nghệ ô tô số',              'AS', N'Chuyên ngành Công nghệ ô tô số',              1, @p9, NULL),
            (@p6, @p8, N'Hệ thống thông tin',             'IS',  N'Chuyên ngành Hệ thống thông tin',             1, @p9, NULL),
            (@p7, @p8, N'Thiết kế đồ hoạ và mỹ thuật số','GD',  N'Chuyên ngành Thiết kế đồ hoạ và mỹ thuật số', 1, @p9, NULL);
            SET IDENTITY_INSERT Majors OFF;";

        await context.Database.ExecuteSqlRawAsync(sql,
            MajorSE, MajorAI, MajorDS, MajorIA, MajorIC, MajorAS, MajorIS, MajorGD,
            DeptCNTT, SeedDate);
    }

    // ─── 3. Semesters ────────────────────────────────────────────────────────

    private static async Task SeedSemestersAsync(AppDbContext context)
    {
        var alreadySeeded = await context.Database
            .SqlQueryRaw<int>("SELECT COUNT(*) AS [Value] FROM Semesters")
            .FirstOrDefaultAsync();

        if (alreadySeeded > 0)
            return;

        var sql = @"
            INSERT INTO Semesters (Id, Name, Code, StartDate, EndDate, AcademicYear, Description, CreatedAt, UpdatedAt)
            VALUES
            (100, N'Học kỳ Thu 2024', 'FA24', '2024-09-01T00:00:00', '2024-12-31T23:59:59', '2024-2025', N'Học kỳ kết thúc', @p0, NULL),
            (101, N'Học kỳ Xuân 2025', 'SP25', '2025-01-01T00:00:00', '2025-05-31T23:59:59', '2024-2025', N'Học kỳ hiện tại', @p0, NULL),
            (102, N'Học kỳ Hè 2025', 'SU25', '2025-06-01T00:00:00', '2025-08-31T23:59:59', '2024-2025', N'Học kỳ sắp tới', @p0, NULL);";

        await context.Database.ExecuteSqlRawAsync(sql, SeedDate);
    }

    // ─── 4. Users ────────────────────────────────────────────────────────────

    private static async Task SeedUsersAsync(AppDbContext context)
    {
        var alreadySeeded = await context.Database
            .SqlQueryRaw<int>("SELECT COUNT(*) AS [Value] FROM Users")
            .FirstOrDefaultAsync();

        if (alreadySeeded > 0)
            return;

        // Firebase Emulator test users (matching Firebase Emulator default test accounts)
        var sql = @"
            -- Test Students
            INSERT INTO Users (
                Id, FirebaseUid, Email, FullName, FirstName, LastName, 
                PhoneNumber, Avatar, DateOfBirth, Gender, Address, City, 
                MajorId, Status, Role, IsActive, CreatedAt, UpdatedAt
            )
            VALUES
            ('10000000-0000-0000-0000-000000000001', 'emulator-test-user-1', 'sv001@student.uni.edu.vn', N'Nguyễn Văn An', 'Văn', 'An', '0123456789', NULL, '2005-01-15T00:00:00Z', 1, N'123 Đường ABC, Hà Nội', 'Hà Nội', 1, 1, 2, 1, @p0, NULL),
            ('10000000-0000-0000-0000-000000000002', 'emulator-test-user-2', 'sv002@student.uni.edu.vn', N'Trần Thị Bình', 'Thị', 'Bình', '0987654321', NULL, '2005-03-20T00:00:00Z', 2, N'456 Đường DEF, Hà Nội', 'Hà Nội', 1, 1, 2, 1, @p0, NULL),
            ('10000000-0000-0000-0000-000000000003', 'emulator-test-user-3', 'sv003@student.uni.edu.vn', N'Lê Văn Côi', 'Văn', 'Côi', '0912345678', NULL, '2005-05-10T00:00:00Z', 1, N'789 Đường GHI, Hà Nội', 'Hà Nội', 2, 1, 2, 1, @p0, NULL),
            ('10000000-0000-0000-0000-000000000004', 'emulator-test-user-4', 'sv004@student.uni.edu.vn', N'Hoàng Minh Đức', 'Minh', 'Đức', '0856789012', NULL, '2005-07-25T00:00:00Z', 1, N'321 Đường JKL, Hà Nội', 'Hà Nội', 2, 1, 2, 1, @p0, NULL),
            
            -- Test Mentor
            ('30000000-0000-0000-0000-000000000001', 'emulator-mentor-1', 'mentor@uni.edu.vn', N'TS. Trần Minh Tuấn', 'Minh', 'Tuấn', '0934567890', NULL, '1980-05-30T00:00:00Z', 1, N'999 Đường XYZ, Hà Nội', 'Hà Nội', NULL, 1, 3, 1, @p0, NULL);";

        await context.Database.ExecuteSqlRawAsync(sql, SeedDate);
    }

    // ─── 5. Student Groups ───────────────────────────────────────────────────

    private static async Task SeedGroupsAsync(AppDbContext context)
    {
        var alreadySeeded = await context.Database
            .SqlQueryRaw<int>("SELECT COUNT(*) AS [Value] FROM Groups")
            .FirstOrDefaultAsync();

        if (alreadySeeded > 0)
            return;

        // Use the seeded user IDs
        var user1Id = Guid.Parse("10000000-0000-0000-0000-000000000001"); // Nguyễn Văn An
        var user2Id = Guid.Parse("10000000-0000-0000-0000-000000000002"); // Trần Thị Bình
        var user3Id = Guid.Parse("10000000-0000-0000-0000-000000000003"); // Lê Văn Côi
        var user4Id = Guid.Parse("10000000-0000-0000-0000-000000000004"); // Hoàng Minh Đức

        var sql = @"
            -- Groups
            INSERT INTO Groups (Id, Code, Name, ProjectId, SemesterId, LeaderId, Status, MaxMembers, IsOpenForRequests, CreatedAt, UpdatedAt)
            VALUES
            ('20000000-0000-0000-0000-000000000001', N'GRP001', N'Nhóm 1: Hệ thống quản lý tuyển sinh', NULL, 101, @p0, 1, 5, 1, @p4, NULL),
            ('20000000-0000-0000-0000-000000000002', N'GRP002', N'Nhóm 2: Ứng dụng AI dự đoán', NULL, 101, @p2, 1, 5, 1, @p4, NULL),
            ('20000000-0000-0000-0000-000000000003', N'GRP003', N'Nhóm 3: Blockchain cho y tế', NULL, 101, @p0, 1, 4, 0, @p4, NULL);

            -- Group Members
            -- Role: 0 = Member, 1 = Leader
            -- Status: 0 = Active
            INSERT INTO GroupMembers (Id, GroupId, StudentId, StudentCode, FullName, Role, Status, JoinedAt)
            VALUES
            (NEWID(), '20000000-0000-0000-0000-000000000001', @p0, N'SV001', N'Nguyễn Văn An', 0, 0, @p4),
            (NEWID(), '20000000-0000-0000-0000-000000000001', @p1, N'SV002', N'Trần Thị Bình', 1, 0, @p4),
            (NEWID(), '20000000-0000-0000-0000-000000000002', @p2, N'SV003', N'Lê Văn Côi', 0, 0, @p4),
            (NEWID(), '20000000-0000-0000-0000-000000000002', @p3, N'SV004', N'Hoàng Minh Đức', 1, 0, @p4),
            (NEWID(), '20000000-0000-0000-0000-000000000003', @p0, N'SV001', N'Nguyễn Văn An', 0, 0, @p4);

            -- Group Invitations
            -- Status: 1 = Pending
            INSERT INTO GroupInvitations (Id, GroupId, InviteeId, InviterId, Status, Message, CreatedAt, ExpiresAt)
            VALUES
            (NEWID(), '20000000-0000-0000-0000-000000000001', @p2, @p0, 1, N'Mời bạn tham gia nhóm của chúng tôi', @p4, DATEADD(DAY, 7, @p4)),
            (NEWID(), '20000000-0000-0000-0000-000000000002', @p0, @p2, 1, N'Gia nhập nhóm AI của chúng tôi', @p4, DATEADD(DAY, 7, @p4)),
            (NEWID(), '20000000-0000-0000-0000-000000000003', @p3, @p0, 1, N'Sắp bắt đầu Blockchain project', @p4, DATEADD(DAY, 5, @p4));";

        await context.Database.ExecuteSqlRawAsync(sql, user1Id, user2Id, user3Id, user4Id, SeedDate);
    }
}
