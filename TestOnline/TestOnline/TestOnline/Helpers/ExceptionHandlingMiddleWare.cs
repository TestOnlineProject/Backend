using Newtonsoft.Json;
using System.Net;

namespace TestOnline.Helpers
{
    public class ExceptionHandlingMiddleware
    {
        public RequestDelegate _requestDelegate;
        public readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception e)
            {
                await HandleException(context, e);
            }

        }

        private Task HandleException(HttpContext context, Exception ex)
        {
            _logger.LogError(ex.ToString());
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var errorMessage =
                new
                {
                    Message = ex.Message,
                    Code = "System_Error",
                    StatusCode = context.Response.StatusCode
                };

            var customResponse = JsonConvert.SerializeObject(errorMessage);

            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(customResponse);
        }
    }
}
