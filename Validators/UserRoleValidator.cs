using FluentValidation;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;
using spm_backend.DTOs.UserRole;

namespace spm_backend.Validators;

public class CreateUserRoleValidator :  AbstractValidator<CreateUserRoleDto>
{
    private readonly AppDbContext _context;
    public CreateUserRoleValidator(AppDbContext context)
    {
        _context = context;
        
        RuleFor(x => x.RoleID)
            .GreaterThan(0)
            .WithMessage("RoleID must be greater than 0.");
        
        RuleFor(x => x.UserID)
            .GreaterThan(0)
            .WithMessage("UserID must be greater than 0.");

        RuleFor(x => x.RoleID)
            .MustAsync(async (roleID, cancellation) =>
                await _context.Roles
                    .AnyAsync(x => x.RoleID == roleID, cancellation))
            .WithMessage("Selected Role does not exist.");
        
        RuleFor(x => x.UserID)
            .MustAsync(async (userID, cancellation) =>
                await _context.Users
                    .AnyAsync(x => x.UserID == userID, cancellation))
            .WithMessage("Selected User does not exist.");
    }
}

public class UpdateUserRoleValidator :  AbstractValidator<UpdateUserRoleDto>
{
    private readonly AppDbContext _context;
    public UpdateUserRoleValidator(AppDbContext context)
    {
        _context = context;
        
        RuleFor(x => x.RoleID)
            .GreaterThan(0)
            .WithMessage("RoleID must be greater than 0.");
        
        RuleFor(x => x.UserID)
            .GreaterThan(0)
            .WithMessage("UserID must be greater than 0.");
        
        RuleFor(x => x.RoleID)
            .MustAsync(async (roleID, cancellation) =>
                await _context.Roles
                    .AnyAsync(x => x.RoleID == roleID, cancellation))
            .WithMessage("Selected Role does not exist.");
        
        RuleFor(x => x.UserID)
            .MustAsync(async (userID, cancellation) =>
                await _context.Users
                    .AnyAsync(x => x.UserID == userID, cancellation))
            .WithMessage("Selected User does not exist.");
    }
}