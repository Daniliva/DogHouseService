using System.Net;
using System.Text.Json;

namespace DogHouse.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;


        public async Task Invoke(HttpContext ctx)
        {
            try
            {
                await _next(ctx);
            }
            catch (Exception ex)
            {
                ctx.Response.ContentType = "application/json";
                switch (ex)
                {
                    case ArgumentException _:
                        ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest; break;
                    case InvalidOperationException _:
                        ctx.Response.StatusCode = (int)HttpStatusCode.Conflict; break;
                    default:
                        ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError; break;
                }
                var payload = JsonSerializer.Serialize(new { error = ex.Message });
                await ctx.Response.WriteAsync(payload);
            }
        }
    }
}
