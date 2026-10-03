using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface ICacheService
    {
        Task<string?> GetAsync(string cachekey, CancellationToken ct = default);
        Task SetAsync(string cachekey, object cacheValue, TimeSpan TimeToLive, CancellationToken ct = default);
    }
}
