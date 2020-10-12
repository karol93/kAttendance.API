using System.Collections.Generic;

namespace kAttendance.Core.Domain
{
   public class Person : BaseEntity
   {
      public string FullName { get; set; }
      public int Year { get; set; }
      public string Email { get; set; }
      public string PhoneNumber { get; set; }
      public int GroupId { get; set; }
      public Group Group { get; set; }
      public ICollection<PersonAttendance> PersonAttendances { get; set; }
   }
}
