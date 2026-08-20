using FluentValidation;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;
using spm_backend.DTOs.User;

namespace spm_backend.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    private readonly AppDbContext _context;
    public CreateUserValidator(AppDbContext context)
    {
        _context = context;
        
        RuleFor(x => x.UserTypeID)
            .GreaterThan(0)
            .WithMessage("UserTypeID must be greater than 0.");
        
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .MaximumLength(150)
            .WithMessage("Full name cannot exceed 150 characters.");
        
        RuleFor(x => x.UserCode)
            .MaximumLength(100)
            .WithMessage("User code cannot exceed 100 characters.");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Please enter a valid email address.")
            .MaximumLength(150)
            .WithMessage("Email cannot exceed 150 characters.");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(255)
            .WithMessage("Password cannot exceed 255 characters.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one number.")
            .Matches(@"[@#$&%]")
            .WithMessage("Password must contain at least one special character (@, #, $, &, %).");
        
        RuleFor(x => x.MobileNumber)
            .NotEmpty()
            .WithMessage("Mobile number is required.")
            .Matches(@"^[0-9]{10}$")
            .WithMessage("Mobile number must contain 10 digits.");
        
        RuleFor(x => x.ProfilePicturePath)
            .MaximumLength(500)
            .WithMessage("Profile picture path is limited to 500 characters.");
        
        // Data Exists Validation
        RuleFor(x => x.UserTypeID)
            .MustAsync(async (userTypeID, cancellation) =>
            {
                return await _context.UserTypes
                    .AnyAsync(x => x.UserTypeID == userTypeID, cancellation);
            })
            .WithMessage("Selected User type does not exist.");
    }
}

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    private readonly AppDbContext _context;
    public UpdateUserValidator(AppDbContext context)
    {
        _context = context;
        
        RuleFor(x => x.UserTypeID)
            .GreaterThan(0)
            .WithMessage("UserTypeID must be greater than 0.");
        
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .MaximumLength(150)
            .WithMessage("Full name cannot exceed 150 characters.");
        
        RuleFor(x => x.UserCode)
            .MaximumLength(100)
            .WithMessage("User code cannot exceed 100 characters.");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Please enter a valid email address.")
            .MaximumLength(150)
            .WithMessage("Email cannot exceed 150 characters.");
        
        RuleFor(x => x.Password)
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(255)
            .WithMessage("Password cannot exceed 255 characters.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one number.")
            .Matches(@"[@#$&%]")
            .WithMessage("Password must contain at least one special character (@, #, $, &, %).")
            .When(x => !string.IsNullOrWhiteSpace(x.Password));
        
        RuleFor(x => x.MobileNumber)
            .NotEmpty()
            .WithMessage("Mobile number is required.")
            .Matches(@"^[0-9]{10}$")
            .WithMessage("Mobile number must contain 10 digits.");
        
        RuleFor(x => x.ProfilePicturePath)
            .MaximumLength(500)
            .WithMessage("Profile picture path is limited to 500 characters.");
        
        RuleFor(x => x.UserTypeID)
            .MustAsync(async (userTypeID, cancellation) =>
            {
                return await _context.UserTypes
                    .AnyAsync(x => x.UserTypeID == userTypeID, cancellation);
            })
            .WithMessage("Selected User type does not exist.");
    }
}