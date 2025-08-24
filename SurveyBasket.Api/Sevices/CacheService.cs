
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SurveyBasket.Api.Sevices
{
    public class CacheService(IDistributedCache distributedCache) : ICacheService
    {
        private readonly IDistributedCache _distributedCache = distributedCache;

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            var value = await _distributedCache.GetStringAsync(key);
           return string.IsNullOrEmpty(value) ? null :
             JsonSerializer.Deserialize<T>(value);
        }
        public async Task SetAsync(string key, object value, TimeSpan? expiration = null)
        {
            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), 
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(60)
                });
        }
        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

      
    }
}
