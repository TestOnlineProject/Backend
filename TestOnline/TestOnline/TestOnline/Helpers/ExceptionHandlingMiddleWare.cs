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
                //switch(context.Response.StatusCode)
                //{
                //    case 400:
                //        context.Response.Redirect("/Error400");
                //        break;
                //    case 404:
                //        context.Response.Redirect("/Error404");
                //        break;
                //    case 500:
                //        context.Response.Redirect("/Error500");
                //        break;
                //    default:
                //        break;
                //}
            }
            catch (Exception e)
            {
                await HandleException(context, e);
            }

        }

        private Task HandleException(HttpContext context, Exception ex)
        {
            _logger.LogError(ex.ToString());
            var errorMessage =
                new
                {
                    Message = ex.Message,
                    Code = "system_error",
                    StatusCode = context.Response.StatusCode
                };

            var customResponse = JsonConvert.SerializeObject(errorMessage);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(customResponse);
        }
    }
}
