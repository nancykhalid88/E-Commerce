using E_Commerce.Domain.Contracts;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IDatabase _database;
        public CacheRepository(IConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();

        }
        public async Task<string?> GetAsync(string cachekey, CancellationToken ct = default)
        {
            var value = await _database.StringGetAsync(cachekey);
            return value.IsNullOrEmpty ? null : value.ToString();
        }

        public Task SetAsync(string cachekey, string cacheValue, TimeSpan TimeToLive, CancellationToken ct = default)
        => _database.StringSetAsync(cachekey, cacheValue, TimeToLive);
    }
}
