using FluentValidation;
using spm_backend.DTOs.UserRole;

namespace spm_backend.Validators;

public class CreateUserRoleValidator :  AbstractValidator<CreateUserRoleDto>
{
    public CreateUserRoleValidator()
    {
        RuleFor(x => x.RoleID)
            .GreaterThan(0)
            .WithMessage("RoleID must be greater than 0.");
        
        RuleFor(x => x.UserID)
            .GreaterThan(0)
            .WithMessage("UserID must be greater than 0.");
    }
}

public class UpdateUserRoleValidator :  AbstractValidator<UpdateUserRoleDto>
{
    public UpdateUserRoleValidator()
    {
        RuleFor(x => x.RoleID)
            .GreaterThan(0)
            .WithMessage("RoleID must be greater than 0.");
        
        RuleFor(x => x.UserID)
            .GreaterThan(0)
            .WithMessage("UserID must be greater than 0.");
    }
}