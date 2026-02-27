using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;

namespace UniThesis.Persistence.Seeds;

/// <summary>
/// Creates matching user accounts in the Firebase Auth Emulator so that
/// every user seeded by <see cref="LoadTestDataSeeder"/> can actually sign in.
/// <para>
/// Requires <c>FIREBASE_AUTH_EMULATOR_HOST</c> to be set (done by Infrastructure DI).
/// Idempotent — silently skips users that already exist.
/// </para>
/// </summary>
public static class FirebaseEmulatorSeeder
{
    private const int ConcurrencyLimit = 20; // Avoid overwhelming the emulator

    public static async Task SeedAsync(ILogger? logger = null)
    {
        var auth = FirebaseAuth.DefaultInstance;
        if (auth == null)
        {
            logger?.LogWarning("FirebaseAuth not initialized. Skipping emulator user seeding.");
            return;
        }

        logger?.LogInformation("Seeding Firebase Emulator users...");

        var users = BuildUserList();
        var created = 0;
        var skipped = 0;

        // Process in batches to limit concurrency
        for (var i = 0; i < users.Count; i += ConcurrencyLimit)
        {
            var batch = users.Skip(i).Take(ConcurrencyLimit);
            var tasks = batch.Select(async u =>
            {
                try
                {
                    await auth.CreateUserAsync(new UserRecordArgs
                    {
                        Uid = u.FirebaseUid,
                        Email = u.Email,
                        Password = LoadTestDataSeeder.DefaultPassword,
                        DisplayName = u.DisplayName,
                        EmailVerified = true
                    });
                    Interlocked.Increment(ref created);
                }
                catch (FirebaseAuthException ex) when (ex.Message.Contains("DUPLICATE_LOCAL_ID") ||
                                                        ex.Message.Contains("EMAIL_EXISTS") ||
                                                        ex.AuthErrorCode == AuthErrorCode.UidAlreadyExists ||
                                                        ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
                {
                    Interlocked.Increment(ref skipped);
                }
            });

            await Task.WhenAll(tasks);
        }

        logger?.LogInformation(
            "Firebase Emulator seeding complete: {Created} created, {Skipped} already existed.",
            created, skipped);
    }

    private static List<(string FirebaseUid, string Email, string DisplayName)> BuildUserList()
    {
        var list = new List<(string, string, string)>();

        for (var i = 1; i <= 5; i++) // Admins
            list.Add((LoadTestDataSeeder.AdminFirebaseUid(i), LoadTestDataSeeder.AdminEmail(i), $"Admin LoadTest {i}"));

        for (var i = 1; i <= 20; i++) // Evaluators
            list.Add((LoadTestDataSeeder.EvaluatorFirebaseUid(i), LoadTestDataSeeder.EvaluatorEmail(i), $"Evaluator LoadTest {i}"));

        for (var i = 1; i <= 50; i++) // Mentors
            list.Add((LoadTestDataSeeder.MentorFirebaseUid(i), LoadTestDataSeeder.MentorEmail(i), $"Mentor LoadTest {i}"));

        for (var i = 1; i <= 925; i++) // Students
            list.Add((LoadTestDataSeeder.StudentFirebaseUid(i), LoadTestDataSeeder.StudentEmail(i), $"Student LoadTest {i}"));

        return list;
    }
}
