using System.Collections.Generic;

namespace kAttendance.Core.Domain
{
    public class Group : BaseEntity
    {
      public string Name { get; set; }
       public IEnumerable<Attendance> Attendances { get; set; }
       public IEnumerable<Person> People { get; set; }
   }
}
