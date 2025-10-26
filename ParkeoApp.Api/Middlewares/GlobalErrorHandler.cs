using ParkeoApp.Api.Models.ApiResponses;

namespace ParkeoApp.Api.Middlewares
{
    public class GlobalErrorHandler(ILogger<GlobalErrorHandler> logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var traceId = Guid.NewGuid().ToString();
                string message = ex.Message;

                var detail = $"{traceId} : {message}";
                logger.LogError(ex, detail);

                var error = new ApiErrorResponse(
                    statusCode: context.Response.StatusCode,
                    errormessage: new { traceId, detail }
                    );

                await context.Response.WriteAsJsonAsync(error);
            }
        }
    }

    public static class GlobalErrorHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder builder) => builder.UseMiddleware<GlobalErrorHandler>();
    }
}
