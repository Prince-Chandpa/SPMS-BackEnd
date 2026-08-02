using System.ComponentModel.DataAnnotations;

namespace spm_backend.Models;

public class TaskStatus
{
    [Key]
    public int TaskStatusID { get; set; }
    
    [Required,MaxLength(20)]
    public string TaskStatusName { get; set; } = string.Empty;
    
    [Required,MaxLength(100)]
    public string TaskStatusCssClass { get; set; } =  string.Empty;
    
}