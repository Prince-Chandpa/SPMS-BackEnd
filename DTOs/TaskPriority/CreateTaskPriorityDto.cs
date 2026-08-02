using System.ComponentModel.DataAnnotations;

namespace spm_backend.DTOs.TaskPriority;

public class CreateTaskPriorityDto
{
    [Required]
    [MaxLength(20)]
    public string TaskPriorityName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string TaskPriorityCssClass { get; set; } =  string.Empty;
    
    public bool IsActive { get; set; } = true;
}