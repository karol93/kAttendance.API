using kAttendance.Infrastructure.Extensions;
using kAttendance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace kAttendance.Infrastructure.Filters
{

   public class ModelValidationFilter : IActionFilter
   {
      public void OnActionExecuted(ActionExecutedContext context)
      {
    
      }

      public void OnActionExecuting(ActionExecutingContext context)
      {
         var modelArgument = context.ActionArguments.SingleOrDefault(kv => kv.Value is BaseModel);
         if (modelArgument.Key == null && modelArgument.Value == null)
         {
            context.Result = new BadRequestObjectResult("Nieprawidłowe żądanie");
            return;
         }

         if (context.ModelState.IsValid == false)
         {
            context.Result = new BadRequestObjectResult(context.ModelState.GetErrors());
            return;
         }
      }
   }
}
