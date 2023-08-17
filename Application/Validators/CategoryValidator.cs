using Application.DataTransferObjects;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class CategoryCreateValidator : AbstractValidator<CategoryCreateDto>
    {
        public CategoryCreateValidator()
        {
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be null or empty");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be null or empty");

        }
    }

    public class CategoryUpdateValidator : AbstractValidator<CategoryUpdateDto>
    {
        public CategoryUpdateValidator()
        {
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be null or empty");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be null or empty");

        }
    }
}
