using E_Commerce.Application.Contracts;
using E_Commerce.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using System.Text.Json;

namespace E_Commerce.API.Attributes
{
    public class RedisCacheAttribute : ActionFilterAttribute
    {
        private readonly int _durationInSec;

        public RedisCacheAttribute(int durationInSec)
        {
            _durationInSec = durationInSec;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Get Cashe Service From Container [Not Injection Direct Into Constructor]
            var CachedService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var CacheKey = CreateCasheKey(context.HttpContext.Request);
            var Cached = await CachedService.GetAsync(CacheKey);

            // If Data Exists InCashe => Get From Cashe And Skip Endpoint
            if (!string.IsNullOrEmpty(Cached)) { 
                context.Result = new ContentResult
                {
                    Content = Cached,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;

                }
            //If Not Exists => Execute Endpoint And Store Result In Cashe If Result Is Ok
            var executed = await next.Invoke();
            if (executed.Result is OkObjectResult { Value: not null } ok)
                await CachedService.SetAsync(CacheKey, ok.Value, TimeSpan.FromSeconds(_durationInSec));
        
        }

        private string CreateCasheKey(HttpRequest request)
        {
            //api/products?page=1&size=10/
            //api/products?page=1&size=10/ user 01
            //    //api/products?page=1&size=10/ user 01
            var key = new StringBuilder();
            key.Append(request.Path).Append('?');
            foreach (var (k, v) in request.Query.OrderBy(q => q.Key))
                key.Append(k).Append('=').Append(v).Append('&');
            return key.ToString();
        }
    }
}
