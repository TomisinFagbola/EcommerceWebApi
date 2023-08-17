using Application.DataTransferObjects;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class UserValidator : AbstractValidator<UserCreateDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be null or empty");
            RuleFor(x => x.Email)
                 .Matches(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$").WithMessage("Enter a valid Email Address");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName cannot be null or empty");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName cannot be null or empty");

        }
    }

    public class ResetPassowordValidator : AbstractValidator<ResetPasswordDto>
    { 
        public ResetPassowordValidator()
        {
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password cannot be null or empty");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm Password cannot be null or empty").When(x => !string.IsNullOrWhiteSpace(x.ConfirmPassword));
        }
    }
}
