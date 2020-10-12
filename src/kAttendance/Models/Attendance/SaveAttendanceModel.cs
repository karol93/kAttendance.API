using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace kAttendance.Models.Attendance
{
    public class SaveAttendanceModel : BaseModel
   {
       public DateTime Date { get; set; }
       public IEnumerable<int> PeopleIds { get; set; }
   }

   public class SaveAttendanceModelValidator : AbstractValidator<SaveAttendanceModel>
   {
      public SaveAttendanceModelValidator()
      {
         RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Pole data jest wymagane.");
         RuleFor(x => x.PeopleIds)
            .Must(p => p != null && p.Any()).WithMessage("Nie wybrano żadnych osób.");
      }
   }
}
