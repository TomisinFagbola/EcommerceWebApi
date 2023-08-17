using Application.DataTransferObjects;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class CatalogueCreateValidator : AbstractValidator<CatalogueCreateDto>
    {
        public CatalogueCreateValidator()
        { 
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be null or empty");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be null or empty");

        }
    }
}
