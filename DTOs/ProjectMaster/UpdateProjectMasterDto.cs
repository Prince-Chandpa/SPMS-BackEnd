using System.ComponentModel.DataAnnotations;

namespace spm_backend.DTOs.ProjectMaster;

public class UpdateProjectMasterDto
{
    [Required]
    [MaxLength(200)] 
    public string ProjectTitle { get; set; } =  string.Empty;
    
    public string Description { get; set; } =  string.Empty;
    
    public bool IsActive { get; set; } = true;
}