using kAttendance.Infrastructure.Filters;
using kAttendance.Models.Export;
using kAttendance.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace kAttendance.Controllers
{
   public class ExportController : Controller
   {
      private readonly IExportService _exportService;
      public ExportController(IExportService exportService) => _exportService = exportService;

      [Route("api/groups/{groupId}/[controller]/{dateFrom}/{dateTo}")]
      [ServiceFilter(typeof(ModelValidationFilter))]
      public IActionResult GetAttendancesForGroup(int groupId, ExportModel model)
      {
         if (!ModelState.IsValid)
            return BadRequest();

         var bytes = _exportService.ExportAttendance(groupId, model.DateFrom, model.DateTo);

         const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
         HttpContext.Response.ContentType = contentType;
         var result = new FileContentResult(bytes, contentType)
         {
            FileDownloadName = "export.xlsx"
         };

         return result;
      }
   }
}
