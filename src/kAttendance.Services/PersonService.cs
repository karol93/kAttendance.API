using AutoMapper;
using kAttendance.Core.Domain;
using kAttendance.Infrastructure.DTO;
using kAttendance.Infrastructure.EntityFramework;
using kAttendance.Infrastructure.Exceptions;
using kAttendance.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace kAttendance.Services
{
   public class PersonService : IPersonService
   {
      private readonly ApplicationDbContext _context;
      private readonly IMapper _mapper;

      public PersonService(ApplicationDbContext context, IMapper mapper)
      {
         _context = context;
         _mapper = mapper;
      }

      public IEnumerable<PersonDto> GetPersonByGroupId(int id)
      {
         var people = _context.People.Where(p => p.GroupId == id);
         return _mapper.Map<IEnumerable<Person>, IEnumerable<PersonDto>>(people);
      }

      public PersonDto CreatePerson(string fullName, int year, int groupId, string email, string phoneNumber)
      {
         var group = _context.Groups.Find(groupId);
         if (group == null)
            throw new ServiceException("Nie odnaleziono wskazanej grupy.");
         Person person = new Person()
         {
            Group = group,
            FullName = fullName,
            Year = year,
            Email = email,
            PhoneNumber = phoneNumber
         };
         _context.People.Add(person);
         _context.SaveChanges();
         return _mapper.Map<Person, PersonDto>(person);
      }

      public void Update(int id, string fullName, int year, string email, string phoneNumber)
      {
         var person = _context.People.Find(id);
         if (person == null)
            throw new ServiceException("Nie odnaleziono wskazanej osoby.");

         person.FullName = fullName;
         person.Year = year;
         person.Email = email;
         person.PhoneNumber = phoneNumber;

         _context.SaveChanges();
      }

      public PersonDto FindByIdAndGroupId(int id, int groupId)
      {
         var person = _context.People.Include(p=>p.Group).FirstOrDefault(p => p.Id == id && p.GroupId == groupId);
         return _mapper.Map<Person, PersonDto>(person);
      }

      public PersonDto FindById(int id)
      {
         var person = _context.People.Find(id);
         return _mapper.Map<Person, PersonDto>(person);
      }

      public void Delete(int id)
      {
         var person = _context.People.Find(id);
         if (person == null)
            throw new ServiceException("Nie odnaleziono wskazanej osoby.");
         _context.People.Remove(person);
         _context.SaveChanges();
      }

      public IEnumerable<PersonDto> GetPeopleByAttendanceId(int id)
      {
         var people = _context.People.Where(p => p.PersonAttendances.Where(a => a.AttendanceId == id) != null);
         return _mapper.Map<IEnumerable<Person>, IEnumerable<PersonDto>>(people);
      }
   }
}
