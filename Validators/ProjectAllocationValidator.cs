using FluentValidation;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;
using spm_backend.DTOs.ProjectAllocation;

namespace spm_backend.Validators;

public class CreateProjectAllocationValidator : AbstractValidator<CreateProjectAllocationDto>
{
    private readonly AppDbContext _context;
    public CreateProjectAllocationValidator(AppDbContext context)
    {
        _context = context;
        
        RuleFor(x => x.ProjectID)
            .GreaterThan(0)
            .WithMessage("ProjectID must be greater than 0.");
        
        RuleFor(x => x.StudentID)
            .GreaterThan(0)
            .WithMessage("StudentID must be greater than 0.");
        
        RuleFor(x => x.FacultyID)
            .GreaterThan(0)
            .WithMessage("FacultyID must be greater than 0.");
        
        RuleFor(x => x.AssignedDate)
            .NotEmpty()
            .WithMessage("Assigned date is required.");
        
        RuleFor(x => x.ProjectStartDate)
            .NotEmpty()
            .WithMessage("Project start date is required.");
        
        RuleFor(x => x.ProjectEndDate)
            .NotEmpty()
            .WithMessage("Project end date is required.");
        
        RuleFor(x => x)
            .Must(x => x.ProjectStartDate <= x.ProjectEndDate)
            .WithMessage("Project end date must be greater than or equal to project start date.");
        
        RuleFor(x => x.TotalTasksGiven)
            .NotEmpty()
            .WithMessage("Total tasks given is required.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total tasks given cannot be negative.");
        
        RuleFor(x => x.TotalCompletedTasks)
            .NotEmpty()
            .WithMessage("Total tasks completed is required.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total completed tasks cannot be negative.");
        
        RuleFor(x => x)
            .Must(x => x.TotalTasksGiven == 0 ? x.ProgressPercentage == 0 : x.ProgressPercentage >= 0 && x.ProgressPercentage <= 100)
            .WithMessage("Progress percentage must be between 0 and 100.");
        
        RuleFor(x => x.OverAllGrade)
            .MaximumLength(1)
            .WithMessage("Overall grade must be a single character.")
            .Must(grade => string.IsNullOrWhiteSpace(grade) || new[] {"A","B","C","D","F"}.Contains(grade.ToUpper()))
            .WithMessage("Overall grade must be A, B, C, D, or F.");

        RuleFor(x => x.ProjectID)
            .MustAsync(async (projectID, cancellation) =>
                await _context.ProjectMasters
                    .AnyAsync(x => x.ProjectMasterID == projectID, cancellation))
            .WithMessage("Selected Project does not exist.");
        
        RuleFor(x => x.StudentID)
            .MustAsync(async (studentID, cancellation) =>
                await _context.Users
                    .AnyAsync(x => x.UserID == studentID && x.UserType.UserTypeName == "Student", cancellation))
            .WithMessage("Selected Student does not exist.");
        
        RuleFor(x => x.FacultyID)
            .MustAsync(async (facultyID, cancellation) =>
                await _context.Users
                    .AnyAsync(x => x.UserID == facultyID && x.UserType.UserTypeName == "Faculty", cancellation))
            .WithMessage("Selected Faculty does not exist.");
    }
}

public class UpdateProjectAllocationValidator : AbstractValidator<UpdateProjectAllocationDto>
{
    private readonly AppDbContext _context;
    public UpdateProjectAllocationValidator(AppDbContext context)
    {
        _context = context;
        
        RuleFor(x => x.ProjectID)
            .GreaterThan(0)
            .WithMessage("ProjectID must be greater than 0.");
        
        RuleFor(x => x.StudentID)
            .GreaterThan(0)
            .WithMessage("StudentID must be greater than 0.");
        
        RuleFor(x => x.FacultyID)
            .GreaterThan(0)
            .WithMessage("FacultyID must be greater than 0.");
        
        RuleFor(x => x.AssignedDate)
            .NotEmpty()
            .WithMessage("Assigned date is required.");
        
        RuleFor(x => x.ProjectStartDate)
            .NotEmpty()
            .WithMessage("Project start date is required.");
        
        RuleFor(x => x.ProjectEndDate)
            .NotEmpty()
            .WithMessage("Project end date is required.");
        
        RuleFor(x => x)
            .Must(x => x.ProjectStartDate <= x.ProjectEndDate)
            .WithMessage("Project end date must be greater than or equal to project start date.");
        
        RuleFor(x => x)
            .Must(x => x.AssignedDate <= x.ProjectStartDate)
            .WithMessage("Assigned date must be before or equal to project start date.");
        
        RuleFor(x => x.TotalTasksGiven)
            .NotEmpty()
            .WithMessage("Total tasks given is required.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total tasks given cannot be negative.");
        
        RuleFor(x => x.TotalCompletedTasks)
            .NotEmpty()
            .WithMessage("Total tasks completed is required.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total completed tasks cannot be negative.");
        
        RuleFor(x => x)
            .Must(x => x.TotalTasksGiven == 0 ? x.ProgressPercentage == 0 : x.ProgressPercentage >= 0 && x.ProgressPercentage <= 100)
            .WithMessage("Progress percentage must be between 0 and 100.");
        
        RuleFor(x => x.OverAllGrade)
            .MaximumLength(1)
            .WithMessage("Overall grade must be a single character.")
            .Must(grade => string.IsNullOrWhiteSpace(grade) || new[] {"A","B","C","D","F"}.Contains(grade.ToUpper()))
            .WithMessage("Overall grade must be A, B, C, D, or F."); 
        
        RuleFor(x => x.ProjectID)
            .MustAsync(async (projectID, cancellation) =>
                await _context.ProjectMasters
                    .AnyAsync(x => x.ProjectMasterID == projectID, cancellation))
            .WithMessage("Selected Project does not exist.");
        
        RuleFor(x => x.StudentID)
            .MustAsync(async (studentID, cancellation) =>
                await _context.Users
                    .AnyAsync(x => x.UserID == studentID && x.UserType.UserTypeName == "Student", cancellation))
            .WithMessage("Selected Student does not exist.");
        
        RuleFor(x => x.FacultyID)
            .MustAsync(async (facultyID, cancellation) =>
                await _context.Users
                    .AnyAsync(x => x.UserID == facultyID && x.UserType.UserTypeName == "Faculty", cancellation))
            .WithMessage("Selected Faculty does not exist.");
    }
}