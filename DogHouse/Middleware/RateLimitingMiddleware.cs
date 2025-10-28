using System.Collections.Concurrent;
using System.Net;
using Microsoft.Extensions.Options;

namespace DogHouse.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RateLimitingOptions _options;
        private static readonly ConcurrentDictionary<string, SlidingWindow> _buckets = new();


        public RateLimitingMiddleware(RequestDelegate next, IOptions<RateLimitingOptions> options)
        {
            _next = next;
            _options = options.Value;
        }


        public async Task Invoke(HttpContext context)
        {
            var key = "global";
            var bucket = _buckets.GetOrAdd(key, _ => new SlidingWindow(_options.RequestsPerSecond));
            if (!bucket.TryRequest())
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                await context.Response.WriteAsync("Too many requests");
                return;
            }


            await _next(context);
        }


        private class SlidingWindow
        {
            private readonly int _limit;
            private readonly Queue<long> _timestamps = new();
            private readonly object _lock = new();


            public SlidingWindow(int limit) => _limit = limit;


            public bool TryRequest()
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                lock (_lock)
                {
                    while (_timestamps.Count > 0 && now - _timestamps.Peek() > 1000)
                        _timestamps.Dequeue();
                    if (_timestamps.Count >= _limit) return false;
                    _timestamps.Enqueue(now);
                    return true;
                }
            }
        }
    }
}