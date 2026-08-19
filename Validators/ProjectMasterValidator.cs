using FluentValidation;
using spm_backend.DTOs.ProjectMaster;

namespace spm_backend.Validators;

public class CreateProjectMasterValidator : AbstractValidator<CreateProjectMasterDto>
{
    public CreateProjectMasterValidator()
    {
        RuleFor(x => x.ProjectTitle)
            .NotEmpty()
            .WithMessage("Project title name is required.")
            .MaximumLength(200)
            .WithMessage("Project title name cannot exceed 200 characters.");
       
        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("Description cannot exceed 200 characters.");
    }
}

public class UpdateProjectMasterValidator : AbstractValidator<UpdateProjectMasterDto>
{
    public UpdateProjectMasterValidator()
    {
        RuleFor(x => x.ProjectTitle)
            .NotEmpty()
            .WithMessage("Project title name is required.")
            .MaximumLength(200)
            .WithMessage("Project title name cannot exceed 200 characters.");
       
        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("Description cannot exceed 200 characters.");   
    }
}