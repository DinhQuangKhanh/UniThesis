
namespace UniThesis.Persistence.SqlServer.Interceptors
{
    /// <summary>
    /// Helper extensions for IServiceProvider.
    /// </summary>
    internal static class ServiceProviderExtensions
    {
        public static IEnumerable<object?> GetServices(this IServiceProvider provider, Type serviceType)
        {
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);
            return (provider.GetService(enumerableType) as IEnumerable<object>) ?? Enumerable.Empty<object>();
        }
    }
}
