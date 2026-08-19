using FluentValidation;
using spm_backend.DTOs.UserType;

namespace spm_backend.Validators;

public class CreateUserTypeValidator : AbstractValidator<CreateUserTypeDto>
{
    public CreateUserTypeValidator()
    {
        RuleFor(x => x.UserTypeName)
            .NotEmpty()
            .WithMessage("User type name is required.")
            .MaximumLength(50)
            .WithMessage("User type name cannot exceed 50 characters.")
            .Must(name => !name.Any(char.IsDigit))
            .WithMessage("User type name cannot contain digits.");

        RuleFor(x => x.Description)
            .MaximumLength(250)
            .WithMessage("Description cannot exceed 250 characters.");
    }
}

public class UpdateUserTypeValidator : AbstractValidator<UpdateUserTypeDto>
{
    public UpdateUserTypeValidator()
    {
        RuleFor(x => x.UserTypeName)
            .NotEmpty()
            .WithMessage("User type name is required.")
            .MaximumLength(50)
            .WithMessage("User type name cannot exceed 50 characters.")
            .Must(name => !name.Any(char.IsDigit))
            .WithMessage("User type name cannot contain digits.");

        RuleFor(x => x.Description)
            .MaximumLength(250)
            .WithMessage("Description cannot exceed 250 characters.");
    }
}