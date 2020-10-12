using kAttendance.Infrastructure.DTO;
using System.Collections.Generic;

namespace kAttendance.Services.Interfaces
{
   public interface IGroupService
   {
      GroupDto FindById(int id);
      IEnumerable<GroupDto> GetAll();
      GroupDto Add(string name);
      void Update(int id, string name);
      void Delete(int id);
   }
}
