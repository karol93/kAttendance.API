using FluentValidation;

namespace kAttendance.Models.Group
{
    public class SaveGroupModel : BaseModel
   {
       public string Name { get; set; }
    }

   public class SaveGroupModelValidator : AbstractValidator<SaveGroupModel>
   {
      public SaveGroupModelValidator()
      {
         RuleFor(x => x.Name).NotEmpty().WithMessage("Pole nazwa jest wymagane");
      }
   }
}
