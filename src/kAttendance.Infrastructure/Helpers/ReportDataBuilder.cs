
using System.Collections.Generic;

namespace kAttendance.Infrastructure.Helpers
{
    public class ReportDataBuilder
    {
       public string ReportName { get; private set; }
       public string Title { get; private set; }
       public string SubTitle { get; private set; }
       public IEnumerable<int> Days { get; private set; }
       public IEnumerable<(string FullName, int Year, IEnumerable<bool> Attendances)> People { get; private set; }
       public ReportDataBuilder SetTitle(string title)
       {
          Title = title;
          return this;
       }

       public ReportDataBuilder SetReportName(string reportName)
       {
          ReportName = reportName;
          return this;
       }

       public ReportDataBuilder SetSubTitle(string subTitle)
       {
          SubTitle = subTitle;
          return this;
       }

       public ReportDataBuilder SetDays(IEnumerable<int> days)
       {
          Days = days;
          return this;
       }

       public ReportDataBuilder SetPeopleWithAttendance(IEnumerable<(string FullName, int Year, IEnumerable<bool> Attendances)> people)
       {
          People = people;
          return this;
       }
   }
}
