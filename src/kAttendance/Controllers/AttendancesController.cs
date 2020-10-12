using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using kAttendance.Models.Attendance;
using kAttendance.Services.Interfaces;
using kAttendance.Infrastructure.Filters;

namespace kAttendance.Controllers
{
   [Route("api/groups/{groupId}/[controller]")]
   public class AttendancesController : Controller
   {
      private readonly IAttendanceService _attendanceService;
      public AttendancesController(IAttendanceService attendanceService) => _attendanceService = attendanceService;

      [HttpGet]
      public IActionResult GetAttendancesForGroup(int groupId) => Ok(_attendanceService.GetAttendancesByGroupId(groupId));

      [HttpGet("{date:datetime}", Name = "GetAttendanceForGroup")]
      public IActionResult GetAttendanceForGroup(int groupId, DateTime date)
      {
         var attendance = _attendanceService.FindAttendanceByGroupIdAndDate(groupId, date);
         if (attendance == null)
            return NotFound();
         return Ok(attendance);
      }

      [HttpPost]
      [ServiceFilter(typeof(ModelValidationFilter))]
      public IActionResult CreateAttendanceForGroup(int groupId, [FromBody] SaveAttendanceModel model)
      {
         if (!ModelState.IsValid)
            return BadRequest();

         var attendance = _attendanceService.Add(groupId,model.Date,model.PeopleIds.ToList());
         return CreatedAtRoute("GetAttendanceForGroup", new { groupId = groupId, date = model.Date }, attendance);
      }

      [HttpPut("{id}")]
      [ServiceFilter(typeof(ModelValidationFilter))]
      public IActionResult UpdateAttendanceForGroup(int id, int groupId, [FromBody] SaveAttendanceModel model)
      {
         if (!ModelState.IsValid)
            return BadRequest();

         _attendanceService.Update(id, model.PeopleIds.ToList(), model.Date);
         return NoContent();
      }


      [HttpGet("~/api/people/{personId}/[controller]")]
      public IActionResult GetAttendancesForPerson(int personId) => Ok(_attendanceService.GetAttendancesByPersonId(personId));

      [HttpDelete("{id:int}")]
      public IActionResult Delete(int id)
      {
         _attendanceService.Delete(id);
         return NoContent();
      }
   }
}
