using System;

namespace kAttendance.Services.Interfaces
{
   public interface IExportService
   { 
      byte[] ExportAttendance(int groupId, DateTime startDate, DateTime endDate);
   }
}
