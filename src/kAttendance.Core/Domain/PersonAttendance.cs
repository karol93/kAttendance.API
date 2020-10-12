namespace kAttendance.Core.Domain
{
   public class PersonAttendance : BaseEntity
   {
      public int PersonId { get; set; }
      public Person Person { get; set; }
      public int AttendanceId { get; set; }
      public Attendance Attendance { get; set; }
   }
}
