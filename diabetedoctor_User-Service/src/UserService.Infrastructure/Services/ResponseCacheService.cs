using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using UserService.Contract.Infrastructure;

namespace UserService.Infrastructure.Services;

public class ResponseCacheService : IResponseCacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly JsonSerializerSettings _jsonSettings;

    public ResponseCacheService(
        IDistributedCache distributedCache,
        IConnectionMultiplexer connectionMultiplexer)
    {
        _distributedCache = distributedCache;
        _connectionMultiplexer = connectionMultiplexer;

        _jsonSettings = new JsonSerializerSettings
        {
            // Chuyển đổi property C# PascalCase sang camelCase trong JSON
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.None
        };
    }

    public async Task DeleteCacheResponseAsync(string cacheKey)
    {
        await _distributedCache.RemoveAsync(cacheKey);
    }

    public async Task<string?> GetCacheResponseAsync(string cacheKey)
    {
        var cacheResponse = await _distributedCache.GetStringAsync(cacheKey);
        return string.IsNullOrEmpty(cacheResponse) ? null : cacheResponse;
    }

    public async Task<List<T>> GetListAsync<T>(string cacheKey)
    {
        var database = _connectionMultiplexer.GetDatabase();
        var serializedList = await database.StringGetAsync(cacheKey);

        if (serializedList.IsNullOrEmpty)
            return null;

        return JsonConvert.DeserializeObject<List<T>>(serializedList, _jsonSettings);
    }

    public async Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut)
    {
        if (response == null) return;

        var serialized = JsonConvert.SerializeObject(response, _jsonSettings);
        await _distributedCache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = timeOut,
        });
    }

    public async Task SetCacheResponseNoTimeoutAsync(string cacheKey, object response)
    {
        if (response == null) return;

        var serialized = JsonConvert.SerializeObject(response, _jsonSettings);
        await _distributedCache.SetStringAsync(cacheKey, serialized);
    }

    public async Task SetListAsync<T>(string cacheKey, List<T> list, TimeSpan timeOut)
    {
        if (list == null || list.Count == 0) return;

        var database = _connectionMultiplexer.GetDatabase();
        var serializedList = JsonConvert.SerializeObject(list, _jsonSettings);
        await database.StringSetAsync(cacheKey, serializedList, timeOut);
    }
}
