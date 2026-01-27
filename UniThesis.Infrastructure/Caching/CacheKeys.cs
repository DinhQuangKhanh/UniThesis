namespace UniThesis.Infrastructure.Caching
{
    public static class CacheKeys
    {
        public static string ActiveSemester => "semester:active";
        public static string SemesterById(int id) => $"semester:{id}";
        public static string ProjectById(Guid id) => $"project:{id}";
        public static string GroupById(Guid id) => $"group:{id}";
        public static string UserPermissions(Guid userId) => $"user:{userId}:permissions";
        public static string SystemConfig(string key) => $"config:{key}";
        public static string DepartmentList => "departments:all";
        public static string MajorsByDepartment(int departmentId) => $"majors:dept:{departmentId}";
        public static string ProjectStats(int semesterId) => $"stats:project:{semesterId}";
        public static string EvaluationStats(int semesterId) => $"stats:evaluation:{semesterId}";
    }
}
