using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace bridges_structures_service.Utils.StorageProvider
{
    public interface ICacheProvider
    {
        Task<string> GetStringAsync(string key);

        Task SetStringAsync(
            string key,
            string value);

        Task SetStringAsync(
            string key,
            string value,
            DistributedCacheEntryOptions options);
    }
}
