using FluentValidation;
using spm_backend.DTOs.TaskPriority;

namespace spm_backend.Validators;

public class CreateTaskPriorityValidator : AbstractValidator<CreateTaskPriorityDto>
{
    public CreateTaskPriorityValidator()
    {
        RuleFor(x => x.TaskPriorityName)
            .NotEmpty()
            .WithMessage("Task priority name is required.")
            .MaximumLength(20)
            .WithMessage("Task priority name cannot exceed 20 characters.")
            .Must(name => !name.Any(char.IsDigit))
            .WithMessage("Task priority name cannot contain digits.");
        
        RuleFor(x => x.TaskPriorityCssClass)
            .NotEmpty()
            .WithMessage("Task priority css class is required.")
            .MaximumLength(20)
            .WithMessage("Task priority css class cannot exceed 20 characters.")
            .Must(name => !name.Any(char.IsDigit))
            .WithMessage("Task priority css class cannot contain digits.");
    }
}

public class UpdateTaskPriorityValidator : AbstractValidator<UpdateTaskPriorityDto>
{
    public UpdateTaskPriorityValidator()
    {
        RuleFor(x => x.TaskPriorityName)
            .NotEmpty()
            .WithMessage("Task priority name is required")
            .MaximumLength(20)
            .WithMessage("Task priority name cannot exceed 20 characters")
            .Must(name => !name.Any(char.IsDigit))
            .WithMessage("Task priority name cannot contain digits");
        
        RuleFor(x => x.TaskPriorityCssClass)
            .NotEmpty()
            .WithMessage("Task priority css class is required")
            .MaximumLength(20)
            .WithMessage("Task priority css class cannot exceed 20 characters")
            .Must(name => !name.Any(char.IsDigit))
            .WithMessage("Task priority css class cannot contain digits");
    }
}