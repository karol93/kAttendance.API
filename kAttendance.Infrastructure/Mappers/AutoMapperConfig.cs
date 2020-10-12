using AutoMapper;
using kAttendance.Core.Domain;
using kAttendance.Infrastructure.DTO;
using System.Linq;

namespace kAttendance.Infrastructure.Mappers
{
   public class AutoMapperProfile : Profile
   {
      public AutoMapperProfile()
      {
         CreateMap<Attendance, AttendanceDto>()
            .ForMember(d=>d.People, m=>m.MapFrom(d=>d.PersonAttendances.Select(p=>new PersonDto()
            {
               Email = p.Person.Email,
               FullName = p.Person.FullName,
               Id = p.PersonId
            })));
         CreateMap<Group, GroupDto>()
            .ForMember(d=>d.NumberOfPeople, m=>m.MapFrom(d=>d.People.Count()));
         CreateMap<Person, PersonDto>()
            .ForMember(d => d.GroupId, m => m.MapFrom(d => d.Group.Id))
            .ForMember(d => d.GroupName, m => m.MapFrom(d => d.Group.Name));
      }
   }
}
