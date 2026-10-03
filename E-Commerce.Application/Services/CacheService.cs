using E_Commerce.Application.Contracts;
using E_Commerce.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly ICacheRepository _cacheRepository;

        public CacheService(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }
        public async Task<string?> GetAsync(string cachekey, CancellationToken ct = default)
        => await _cacheRepository.GetAsync(cachekey, ct);

        public async Task SetAsync(string cachekey, object cacheValue, TimeSpan TimeToLive, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(cacheValue, new JsonSerializerOptions() { 
                PropertyNamingPolicy=JsonNamingPolicy.CamelCase});

            await _cacheRepository.SetAsync(cachekey, json, TimeToLive, ct); 
        }
        
    }
}
