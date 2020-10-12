using kAttendance.Infrastructure.DTO;
using System;
using System.Collections.Generic;

namespace kAttendance.Services.Interfaces
{
   public interface IAttendanceService
   {
      IEnumerable<AttendanceDto> GetAttendancesByGroupId(int groupId);
      IEnumerable<AttendanceDto> GetAttendancesByPersonId(int personId);
      AttendanceDto FindAttendanceByGroupIdAndDate(int groupId, DateTime date);
      AttendanceDto Add(int groupId, DateTime date, List<int> peopleIds);
      void Delete(int id);
      void Update(int id, List<int> peopleIds, DateTime date);
      AttendanceDto FindById(int id);

   }
}
