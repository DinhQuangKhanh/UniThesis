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
            .FirstOrDefaultAsync();

        if (alreadySeeded > 0)
            return;

        await SeedDepartmentsAsync(context);
        await SeedMajorsAsync(context);
        await SeedSemestersAsync(context);
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
}
