using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace kAttendance.Core.Domain
{
    public class Attendance : BaseEntity
    {
       [Required]
      public DateTime Date { get; set; }
       public int GroupId { get; set; }
       public Group Group { get; set; }

       public virtual ICollection<PersonAttendance> PersonAttendances { get; set; }

   }
}
