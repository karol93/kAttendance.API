using System;
using System.Collections.Generic;

namespace kAttendance.Infrastructure.DTO
{
    public class AttendanceDto
    {
       public int Id { get; set; }
       public DateTime Date { get; set; }
       public IEnumerable<PersonDto> People { get; set; }
    }
}
