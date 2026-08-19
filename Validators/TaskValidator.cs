using FluentValidation;
using spm_backend.DTOs.Task;

namespace spm_backend.Validators;

public class CreateTaskValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.ProjectAllocationID)
            .GreaterThan(0)
            .WithMessage("Project Allocation ID must be greater than 0.");
        
        RuleFor(x => x.TaskStatusID)
            .GreaterThan(0)
            .WithMessage("Task StatusID must be greater than 0.");
        
        RuleFor(x => x.TaskPriorityID)
            .GreaterThan(0)
            .WithMessage("Task PriorityID must be greater than 0.");
        
        RuleFor(x => x.TaskTitle)
            .NotEmpty()
            .WithMessage("Task title is required.")
            .MaximumLength(200)
            .WithMessage("Task title cannot exceed 200 characters.");
        
        RuleFor(x => x.TaskDescription)
            .MaximumLength(250)
            .WithMessage("Task description cannot exceed 250 characters.");
        
        RuleFor(x => x.AssignedScore)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Assigned score must be greater than or equal to 0.");
        
        RuleFor(x => x.EarnedScore)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Earned score must be greater than or equal to 0.");
        
        RuleFor(x => x)
            .Must(x => !x.EarnedScore.HasValue || x.EarnedScore.Value <= x.AssignedScore)
            .WithMessage("Earned score cannot be greater than assigned score.");
        
        RuleFor(x => x.ProgressPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Progress percentage must be between 0 and 100.");

        RuleFor(x => x.TaskAssignedDate)
            .NotEmpty()
            .WithMessage("Task assigned date is required.");
        
        RuleFor(x => x)
            .Must(x => !x.TaskStartDate.HasValue || x.TaskStartDate.Value >= x.TaskAssignedDate)
            .WithMessage("Task start date must be greater than or equal to task assigned date.");
        
        RuleFor(x => x)
            .Must(x => !x.TaskDueDate.HasValue || !x.TaskStartDate.HasValue || x.TaskDueDate.Value >= x.TaskStartDate.Value)
            .WithMessage("Task due date must be greater than or equal to task start date.");
        
        RuleFor(x => x)
            .Must(x => !x.TaskCompletedDate.HasValue || !x.TaskStartDate.HasValue || x.TaskCompletedDate.Value >= x.TaskStartDate.Value)
            .WithMessage("Task completed date must be greater than or equal to task start date.");
        
        RuleFor(x => x)
            .Must(x => !x.TaskCompletedDate.HasValue || !x.TaskDueDate.HasValue || x.TaskCompletedDate.Value >= x.TaskDueDate.Value)
            .WithMessage("Task completed date must be greater than or equal to task due date.");
        
        RuleFor(x => x)
            .Must(x => !x.NextFollowUpDate.HasValue || x.NextFollowUpDate.Value >= x.TaskAssignedDate)
            .WithMessage("Next follow-up date must be greater than or equal to task assigned date.");

        RuleFor(x => x.FacultyRemarks)
            .MaximumLength(500)
            .WithMessage("Faculty remarks cannot exceed 500 characters.");
        
        RuleFor(x => x.StudentRemarks)
            .MaximumLength(500)
            .WithMessage("Student remarks cannot exceed 500 characters.");
    }
}

public class UpdateTaskValidator : AbstractValidator<UpdateTaskDto>
{
    public UpdateTaskValidator()
    {
        RuleFor(x => x.ProjectAllocationID)
            .GreaterThan(0)
            .WithMessage("Project Allocation ID must be greater than 0.");
        
        RuleFor(x => x.TaskStatusID)
            .GreaterThan(0)
            .WithMessage("Task StatusID must be greater than 0.");
        
        RuleFor(x => x.TaskPriorityID)
            .GreaterThan(0)
            .WithMessage("Task PriorityID must be greater than 0.");
        
        RuleFor(x => x.TaskTitle)
            .NotEmpty()
            .WithMessage("Task title is required.")
            .MaximumLength(200)
            .WithMessage("Task title cannot exceed 200 characters.");
        
        RuleFor(x => x.TaskDescription)
            .MaximumLength(250)
            .WithMessage("Task description cannot exceed 250 characters.");
        
        RuleFor(x => x.AssignedScore)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Assigned score must be greater than or equal to 0.");
        
        RuleFor(x => x.EarnedScore)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Earned score must be greater than or equal to 0.");
        
        RuleFor(x => x)
            .Must(x => !x.EarnedScore.HasValue || x.EarnedScore.Value <= x.AssignedScore)
            .WithMessage("Earned score cannot be greater than assigned score.");
        
        RuleFor(x => x.ProgressPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Progress percentage must be between 0 and 100.");

        RuleFor(x => x.TaskAssignedDate)
            .NotEmpty()
            .WithMessage("Task assigned date is required.");
        
        RuleFor(x => x)
            .Must(x => !x.TaskStartDate.HasValue || x.TaskStartDate.Value >= x.TaskAssignedDate)
            .WithMessage("Task start date must be greater than or equal to task assigned date.");
        
        RuleFor(x => x)
            .Must(x => !x.TaskDueDate.HasValue || !x.TaskStartDate.HasValue || x.TaskDueDate.Value >= x.TaskStartDate.Value)
            .WithMessage("Task due date must be greater than or equal to task start date.");
        
        RuleFor(x => x)
            .Must(x => !x.TaskCompletedDate.HasValue || !x.TaskStartDate.HasValue || x.TaskCompletedDate.Value >= x.TaskStartDate.Value)
            .WithMessage("Task completed date must be greater than or equal to task start date.");
        
        RuleFor(x => x)
            .Must(x => !x.TaskCompletedDate.HasValue || !x.TaskDueDate.HasValue || x.TaskCompletedDate.Value >= x.TaskDueDate.Value)
            .WithMessage("Task completed date must be greater than or equal to task due date.");
        
        RuleFor(x => x.FacultyRemarks)
            .MaximumLength(500)
            .WithMessage("Faculty remarks cannot exceed 500 characters.");
        
        RuleFor(x => x.StudentRemarks)
            .MaximumLength(500)
            .WithMessage("Student remarks cannot exceed 500 characters.");  
    }
}