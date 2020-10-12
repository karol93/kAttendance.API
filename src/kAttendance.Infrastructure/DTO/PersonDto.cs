namespace kAttendance.Infrastructure.DTO
{
   public class PersonDto
    {
       public int Id { get; set; }
       public string FullName { get; set; }
       public string Year { get; set; }
       public string Email { get; set; }
       public string PhoneNumber { get; set; }
       public int GroupId { get; set; }
       public string GroupName { get; set; }
    }
}
