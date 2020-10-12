using System.Net;
using kAttendance.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace kAttendance.Infrastructure
{
   public class HttpGlobalExceptionFilter : IExceptionFilter
   {
      private readonly IHostingEnvironment _env;

      public HttpGlobalExceptionFilter(IHostingEnvironment env)
      {
         _env = env;
      }

      public void OnException(ExceptionContext context)
      {
         var exception = context.Exception;
         var exceptionType = exception.GetType();
         var message = "";
         if (exceptionType == typeof(ServiceException))
         {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            message = exception.Message;
         }
         else
         {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            message = "Ups. Coś poszło nie tak.";
         }

         object response;
         if (_env.IsDevelopment())
         {
            response = $"Message: {exception.Message} StackTrace: {exception.StackTrace}";
         }
         else
            response = message;

         var payload = JsonConvert.SerializeObject(response);
         context.HttpContext.Response.ContentType = "application/json";
         context.HttpContext.Response.WriteAsync(payload);
         context.ExceptionHandled = true;
      }
   }
}
