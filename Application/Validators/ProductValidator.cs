using Application.DataTransferObjects;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateValidator()
        {

            RuleFor(x => x.Name).NotEmpty().WithMessage("Title cannot be null or empty");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be null or empty");
            RuleFor(x => x.Summary).NotEmpty().WithMessage("Summary cannot be null or empty");
            RuleFor(x => x.Price).NotEmpty().WithMessage("Price cannot be null or empty");
            RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile cannot be null or empty");
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Category cannot be null or empty");
           
        }
    }
}
