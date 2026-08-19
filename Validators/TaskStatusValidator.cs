using FluentValidation;
using spm_backend.DTOs.TaskStatus;

namespace spm_backend.Validators;

public class CreateTaskStatusValidator : AbstractValidator<CreateTaskStatusDto>
{
    public CreateTaskStatusValidator()
    {
        RuleFor(x => x.TaskStatusName)
            .NotEmpty()
            .WithMessage("Task status name is required.")
            .MaximumLength(20)
            .WithMessage("Task status name cannot exceed 20 characters.")
            .Must(name => !name.Any(char.IsDigit))
            .WithMessage("Task status name cannot contain digits.");
        
        RuleFor(x => x.TaskStatusCssClass)
            .NotEmpty()
            .WithMessage("Task status css class is required.")
            .MaximumLength(100)
            .WithMessage("Task status css class cannot exceed 20 characters.")
            .Must(name => !name.Any(char.IsDigit))
            .WithMessage("Task status css class cannot contain digits.");
    }
}

public class UpdateTaskStatusValidator : AbstractValidator<UpdateTaskStatusDto>
{
    public UpdateTaskStatusValidator()
    {
        RuleFor(x => x.TaskStatusName)
            .NotEmpty()
            .WithMessage("Task status name is required.")
            .MaximumLength(20)
            .WithMessage("Task status name cannot exceed 20 characters.")
            .Must(name => !name.Any(char.IsDigit))
            .WithMessage("Task status name cannot contain digits.");
        
        RuleFor(x => x.TaskStatusCssClass)
            .NotEmpty()
            .WithMessage("Task status css class is required.")
            .MaximumLength(100)
            .WithMessage("Task status css class cannot exceed 20 characters.")
            .Must(name => !name.Any(char.IsDigit))
            .WithMessage("Task status css class cannot contain digits.");
    }
}