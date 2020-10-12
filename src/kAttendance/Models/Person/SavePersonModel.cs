using FluentValidation;
using System;

namespace kAttendance.Models.Person
{
   public class SavePersonModel : BaseModel
   {
      public string FullName { get; set; }
      public int Year { get; set; }
      public string Email { get; set; }
      public string PhoneNumber { get; set; }
   }

   public class SavePersonModelValidator : AbstractValidator<SavePersonModel>
   {
      public SavePersonModelValidator()
      {
         RuleFor(x => x.FullName).NotEmpty().WithMessage("Pole imię i nazwisko jest wymagane.");
         RuleFor(x => x.Year)
            .NotNull().WithMessage("Pole rok jest wymagane.")
            .GreaterThan(1900).WithMessage("Pole rok musi posiadać wartość większą od 1900.")
            .LessThan(DateTime.Now.Year).WithMessage($"Pole rok musi posiadać wartość mniejszą od {DateTime.Now.Year}.");
         RuleFor(x => x.Email).EmailAddress().Unless(x=>string.IsNullOrEmpty(x.Email)).WithMessage("Pole e-mail musi posiadać adres w prawidłowym formacie.");
      }
   }
}
