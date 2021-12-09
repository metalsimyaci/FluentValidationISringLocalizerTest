using Microsoft.Extensions.Localization;

namespace FluentValidation.Console
{
    public class StudentValidator:AbstractValidator<Student>
    {
        public StudentValidator(IStringLocalizer<Student> localizer)
        {
            RuleFor(r => r.Id).GreaterThan(1).WithMessage(m => localizer["IncorrectId"]);
            RuleFor(r => r.Name).NotEmpty().NotNull().WithMessage(m => localizer["NotNullOrNotEmpty"]);
        }
    }
}
