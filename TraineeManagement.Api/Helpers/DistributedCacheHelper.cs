using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TraineeManagement.Api.Helpers
{
    public static class DistributedCacheHelper
    {
        private static readonly DistributedCacheEntryOptions DefaultOptions = new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(10),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };

        public static async Task<T?> GetOrSetAsync<T>(this IDistributedCache cache, string cacheKey,
            Func<Task<T?>> retrieveFromDb, ILogger logger, DistributedCacheEntryOptions? options = null) where T : class
        {
            try
            {
                string? cachedData = await cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    logger.LogInformation("Cache Data found");
                    return JsonSerializer.Deserialize<T>(cachedData);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Redis cache read failed for key {Key}.", cacheKey);
            }
            logger.LogInformation("Falling back to sql database");
            T? data = await retrieveFromDb();
            if (data is null) return null;

            try
            {
                string serializedData = JsonSerializer.Serialize(data);
                await cache.SetStringAsync(cacheKey, serializedData, options ?? DefaultOptions);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Unable to write data to Redis cache for key {Key}.", cacheKey);
            }

            return data;
        }

        public static async Task RemoveCacheAsync(this IDistributedCache cache, string cacheKey, ILogger logger)
        {
            try
            {
                await cache.RemoveAsync(cacheKey);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to invalidate cache key {Key}.", cacheKey);
            }
        }
    }
}
