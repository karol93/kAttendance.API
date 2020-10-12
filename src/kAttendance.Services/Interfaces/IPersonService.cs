using kAttendance.Infrastructure.DTO;
using System.Collections.Generic;

namespace kAttendance.Services.Interfaces
{
   public interface IPersonService
   {
      IEnumerable<PersonDto> GetPersonByGroupId(int id);
      PersonDto CreatePerson(string fullName, int year, int groupId, string email, string phoneNumber);
      PersonDto FindByIdAndGroupId(int id, int groupId);
      PersonDto FindById(int id);
      void Delete(int id);
      IEnumerable<PersonDto> GetPeopleByAttendanceId(int id);
      void Update(int id, string fullName, int year, string email, string phoneNumber);
   }
}
