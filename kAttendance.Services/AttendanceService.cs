using AutoMapper;
using kAttendance.Core.Domain;
using kAttendance.Infrastructure.DTO;
using kAttendance.Infrastructure.EntityFramework;
using kAttendance.Infrastructure.Exceptions;
using kAttendance.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace kAttendance.Services
{

   public class AttendanceService : IAttendanceService
   {
      private readonly ApplicationDbContext _context;
      private readonly IMapper _mapper;

      public AttendanceService(ApplicationDbContext context, IMapper mapper)
      {
         _context = context;
         _mapper = mapper;
      }
      public IEnumerable<AttendanceDto> GetAttendancesByGroupId(int groupId)
      {
         var attendances = _context.Attendances.Where(p => p.GroupId == groupId);
         return _mapper.Map<IEnumerable<Attendance>, IEnumerable<AttendanceDto>>(attendances);
      }

      public IEnumerable<AttendanceDto> GetAttendancesByPersonId(int personId)
      {
         var attendances = _context.Attendances.SelectMany(p => p
            .PersonAttendances.Where(c => c.PersonId == personId)
            .Select(a => a.Attendance));
         return _mapper.Map<IEnumerable<Attendance>, IEnumerable<AttendanceDto>>(attendances);
      }

      public AttendanceDto FindAttendanceByGroupIdAndDate(int groupId, DateTime date)
      {
         var attendance = _context.Attendances
            .Include(p=>p.PersonAttendances)
            .ThenInclude(p=>p.Person)
            .FirstOrDefault(p => p.GroupId == groupId && p.Date.Date == date.Date);
         return _mapper.Map<Attendance, AttendanceDto>(attendance); 
      }

      public AttendanceDto Add(int groupId, DateTime date, List<int> peopleIds)
      {
         if (!peopleIds.Any())
            throw new ServiceException("Co najmniej jedna osoba musi zostać wybrana.");

         var group = _context.Groups.Find(groupId);
         if (group == null)
            throw new ServiceException($"Wskazana grupa do której próbowano dodać obecność nie istnieje.");

         if (IsDuplicated(groupId,date))
            throw new ServiceException(
               "Istnieje już zarejestrowana obecność na ten dzień. Aby dodać ponownie należy usunąć poprzednią.");

         var attendance = new Attendance()
         {
            Group = group,
            Date = date,
         };

         _context.Attendances.Add(attendance);

         CreatePersonAttendance(peopleIds, attendance);

         _context.SaveChanges();

         return _mapper.Map<Attendance, AttendanceDto>(attendance);
      }

      public void Delete(int id)
      {
         var attendance = _context.Attendances.Find(id);
         if (attendance == null)
            throw new ServiceException($"Wskazana obecność nie istnieje.");

         _context.Attendances.Remove(attendance);
         _context.SaveChanges();
      }

      public void Update(int id, List<int> peopleIds, DateTime date)
      {
         var attendance = _context.Attendances.Find(id);
         if (attendance == null)
            throw new ServiceException($"Wskazana obecność nie istnieje.");

         if (!peopleIds.Any())
            throw new ServiceException("Co najmniej jedna osoba musi zostać wybrana.");

         if (IsDuplicated(attendance.GroupId, date))
            throw new ServiceException(
               "Istnieje już zarejestrowana obecność na ten dzień. Aby dodać ponownie należy usunąć poprzednią.");

         attendance.Date = date;

         var personAttendances = _context.PersonAttendances.Where(p => p.AttendanceId == id);
         _context.PersonAttendances.RemoveRange(personAttendances);

         CreatePersonAttendance(peopleIds,attendance);

         _context.SaveChanges();
      }

      public AttendanceDto FindById(int id)
      {
         var attendance = _context.Attendances.Find(id);
         return _mapper.Map<Attendance, AttendanceDto>(attendance);
      }

      private void CreatePersonAttendance(List<int> peopleIds, Attendance attendance)
      {
         foreach (var personId in peopleIds)
         {
            var person = _context.People.FirstOrDefault(p => p.Id == personId && p.GroupId == attendance.GroupId);

            var personAttendance = new PersonAttendance()
            {
               Attendance = attendance,
               Person = person
            };
            _context.PersonAttendances.Add(personAttendance);
         }
      }

      private bool IsDuplicated(int groupId, DateTime date) => _context.Attendances.FirstOrDefault(p => p.GroupId == groupId && p.Date.Date == date.Date) != null;
   }
}
