using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace kAttendance.Infrastructure.Extensions
{
   public static class ModelStateExtensions
   {
      public static string GetErrors(this ModelStateDictionary modelState)
      {
         if (!modelState.IsValid)
         {
            return modelState.SelectMany(state => state.Value.Errors).Aggregate("", (current, error) => current + (error.ErrorMessage + " "));
         }
         return string.Empty;
      }
   }
}
