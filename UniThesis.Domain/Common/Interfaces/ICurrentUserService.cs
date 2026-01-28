namespace UniThesis.Domain.Common.Interfaces
{
    /// <summary>
    /// DEPRECATED: This interface has been moved to UniThesis.Application.Common.Interfaces.
    /// This alias is kept for backward compatibility. Please use the Application layer version.
    /// </summary>
    [Obsolete("Use UniThesis.Application.Common.Interfaces.ICurrentUserService instead. This interface will be removed in a future version.")]
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Email { get; }
        IEnumerable<string> Roles { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
    }
}
