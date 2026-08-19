using FluentValidation;
using spm_backend.DTOs.Role;

namespace spm_backend.Validators;

public class RoleValidator : AbstractValidator<CreateRoleDto>
{
    public RoleValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithMessage("Role name is required")
            .MaximumLength(50)
            .WithMessage("Role name cannot exceed 50 characters")
            .Must(name => !name.Any(char.IsDigit))
            .WithMessage("Role name cannot contain digits");
        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("Description cannot exceed 200 characters");
    }    
}

public class UpdateRoleValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithMessage("Role name is required")
            .MaximumLength(50)
            .WithMessage("Role name cannot exceed 50 characters")
            .Must(name => !name.Any(char.IsDigit))
            .WithMessage("Role name cannot contain digits");
        
        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("Description cannot exceed 200 characters");
    }
}