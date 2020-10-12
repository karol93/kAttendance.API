using kAttendance.Core.Domain;
using kAttendance.Infrastructure.EntityFramework;
using kAttendance.Infrastructure.Exceptions;
using kAttendance.Infrastructure.Helpers;
using kAttendance.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace kAttendance.Services
{
   public class ExportService : IExportService
   {
      private readonly ApplicationDbContext _context;

      public ExportService(ApplicationDbContext context)
      {
         _context = context;
      }

      public byte[] ExportAttendance(int groupId, DateTime startDate, DateTime endDate)
      {
         var group = _context.Groups.FirstOrDefault(x => x.Id == groupId);
         if (group == null)
            throw new ServiceException("Nie odnaleziono wskazanej grupy.");

         var attendancesGroupedByYearAndMonth = GetAttendancesGroupedByYearAndMonth(startDate, endDate, group);

         if(!attendancesGroupedByYearAndMonth.Any())
            throw new ServiceException($"Brak obecności w podanym zakresie dat.");

         List<ReportDataBuilder> reportsDataBuilders = new List<ReportDataBuilder>();
         foreach (var keyValuePair in attendancesGroupedByYearAndMonth)
         {
            IEnumerable<Attendance> attendancesInMonth = keyValuePair.Value;
            int month = keyValuePair.Key.Month;
            int year = keyValuePair.Key.Year;
            DateTime firstAttendanceInMonth = attendancesInMonth.First().Date;
            DateTime lastAttendanceInMonth = attendancesInMonth.Last().Date;

            var people = _context.People.Where(p => p.GroupId == groupId).ToList();

            var reportDataBuilder = new ReportDataBuilder();

            reportDataBuilder
               .SetReportName($"{month}-{year}")
               .SetTitle($"LISTA OBECNOŚCI NA ZAJĘCIACH - TR. SOŁTYSIK grupa {group.Name}")
               .SetSubTitle($"{year}-{month}")
               .SetDays(attendancesInMonth.Select(p => p.Date.Day))
               .SetPeopleWithAttendance(people.Select(p => (p.FullName, p.Year, GetPersonAttendances(p,attendancesInMonth))));

            reportsDataBuilders.Add(reportDataBuilder);
         }

         return ReportGenerator.GetReport(reportsDataBuilders);
      }

      private Dictionary<(int Year, int Month), List<Attendance>> GetAttendancesGroupedByYearAndMonth(
         DateTime startDate, DateTime endDate, Group group)
      {
         return _context.Attendances.Include(p => p.PersonAttendances).ThenInclude(p => p.Person)
            .Where(p => p.GroupId == group.Id && p.Date >= startDate && p.Date <= endDate)
            .OrderBy(p => p.Date).ToList().GroupBy(p => new {p.Date.Year, p.Date.Month})
            .ToDictionary(p => (p.Key.Year, p.Key.Month), p => p.ToList().OrderBy(x => x.Date).ToList());
      }

      private IEnumerable<bool> GetPersonAttendances(Person person, IEnumerable<Attendance> attendances)
      {
         foreach (var attendance in attendances)
         {
            var personAttendances =
               _context.PersonAttendances.FirstOrDefault(
                  p => p.AttendanceId == attendance.Id && p.PersonId == person.Id);
            if (personAttendances != null)
               yield return true;
            else
               yield return false;
         }
      }
   }
}
