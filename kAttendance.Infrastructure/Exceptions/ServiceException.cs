using System;

namespace kAttendance.Infrastructure.Exceptions
{
   public class ServiceException : Exception
   {
      public ServiceException(string message) : base(message)
      {
      }
   }
}
