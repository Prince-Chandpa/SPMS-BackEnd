using System.ComponentModel.DataAnnotations;

namespace spm_backend.DTOs.TaskStatus;

public class UpdateTaskStatusDto
{
    [Required]
    [MaxLength(20)]
    public string TaskStatusName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string TaskStatusCssClass { get; set; } =  string.Empty;
    
    public bool IsActive { get; set; } = true;
}