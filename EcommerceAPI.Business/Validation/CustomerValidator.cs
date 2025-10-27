using FluentValidation;
using EcommerceAPI.Entities;

namespace EcommerceAPI.Business.Validation;

// Swap to a Customer DTO later if you add request models for customers.
public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
    }
}
