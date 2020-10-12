using FluentValidation;
using System;

namespace kAttendance.Models.Export
{
   public class ExportModel : BaseModel
   {
       public DateTime DateFrom { get; set; }
       public DateTime DateTo { get; set; }
    }

   public class ExportModelValidator : AbstractValidator<ExportModel>
   {
      public ExportModelValidator()
      {
         RuleFor(x => x.DateFrom)
            .NotEmpty().WithMessage("Pole data od jest wymagane.");
         RuleFor(x => x.DateTo)
            .NotEmpty().WithMessage("Pole data do jest wymagane.")
            .GreaterThanOrEqualTo(x => x.DateFrom).WithMessage("Pole data do musi być większe lub równe polu data od.");
      }
   }
}
